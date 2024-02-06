using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using PlayerIOClient;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class NetworkManager : MonoBehaviour
{
    #region Singleton
    public static NetworkManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [Header("User")]
    public string UserId;
    public bool IsAdmin;
    public string CurrentRoomId;

    [Header("Parameters")] [SerializeField] private NetworkMode _networkMode;

    [Header("Menu Scene")] public string M_MenuManagerIdentifier;
    
    [Header("Room Selection Scene")] public string RS_MenuManagerIdentifier;
    public string RoomManagerIdentifier;
    
    [Header("Admin or User Scene")]
    public string RoomInterfaceControllerIdentifier;

    private RoomInterfaceController _roomInterfaceController;
    
    #region Getters

    public Connection PioconnectionRoom
    {
        get => _pioconnectionRoom;
        private set => _pioconnectionRoom = value;
    }
    
    #endregion
    
    #region Intern Variables
    // References
    private MenuManager _menuManager;
    private RoomManager _roomManager;

    // Player.io
    private Connection _pioconnectionRoom;
    private Connection _pioconnectionWoz;
    private List<Message> _msgList = new List<Message>();
    
    // Room Types
    private string _roomSelectionType = "Room_Selection";
    private string _wozRoomType = "Room_Wizard_Of_Oz";
    
    // Room IDs
    private string _mainRoomId = "UMONS_Avatar_Lobby";
    #endregion

    private void LookForScriptRefOnSceneChange(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (_menuManager == null)
        {
            try
            {
                _menuManager = GameObject.Find(scene.name == "_Ver 1.0 - Menu" ? M_MenuManagerIdentifier : RS_MenuManagerIdentifier).GetComponent<MenuManager>();
            }
            catch (Exception e)
            {
                Debug.Log("_menuManager not found in that scene");
            }
        }

        if (_roomManager == null)
        {
            try
            {
                _roomManager = GameObject.Find(RoomManagerIdentifier).GetComponent<RoomManager>();
            }
            catch (Exception e)
            {
                Debug.Log("_roomManager not found in that scene");
            }
        }

        if (_roomInterfaceController == null)
        {
            try
            {
                _roomInterfaceController = GameObject.Find(RoomInterfaceControllerIdentifier)
                    .GetComponent<RoomInterfaceController>();
            }
            catch (Exception e)
            {
                Debug.Log("_roomInterfaceController not found in that scene");
            }
        }
    }

    void Start()
    {
        // Toggle event when scene is changing
        SceneManager.sceneLoaded += LookForScriptRefOnSceneChange;
        
        // Look for script ref in current scene
        LookForScriptRefOnSceneChange(SceneManager.GetActiveScene(), LoadSceneMode.Single);

        Application.runInBackground = true;

        // Create a random userid 
        System.Random random = new System.Random();
        UserId = "Guest" + random.Next(0, 10000);

        //ConnectToRoom(_mainRoomId, _roomSelectionType);
    }

    // Get answer from server
    private void FixedUpdate()
    {
        foreach (Message m in _msgList)
        {
            switch (m.Type)
            {
                case "UserConnected":
                    FeedbackManager.Instance.CreateFeedBack("New user connected: " + m.GetString(0),
                        FeedbackType.INFORMATION);
                    break;
                
                #region Woz Room : ADMIN & USER
                case "AskForUserInformation":
                    _pioconnectionWoz.Send("SendUserInfos", IsAdmin);
                    break;
                case "AvatarHeadMove":
                    if (IsAdmin)
                    {
                        throw new Exception("Admin got an order of avatar head movement");
                    }
                    MainManager.Instance.HeadPoseController.HeadPoseUpdateByValue(m.GetDouble(0), m.GetDouble(1), m.GetDouble(2));
                    break;
                case "AvatarBlendShapesMove":
                    if (IsAdmin)
                    {
                        throw new Exception("Admin got an order of avatar key movement");
                    }
                    MainManager.Instance.BlendShapesController.ChangeBlendShapesByDict(JsonConvert.DeserializeObject<Dictionary<string, double>>(m.GetString(0)));
                    break;
                case "AvatarBlendShapesTransition":
                    if (IsAdmin)
                    {
                        throw new Exception("Admin got an order of AvatarBlendShapesTransition");
                    }
                    MainManager.Instance.BlendShapesController.TransitionToDict(JsonConvert.DeserializeObject<Dictionary<string, double>>(m.GetString(0)), 
                        m.GetFloat(1));
                    break;
                case "AvatarPoseTransition":
                    if (IsAdmin)
                    {
                        throw new Exception("Admin got an order of AvatarPoseTransition");
                    }
                    MainManager.Instance.HeadPoseController.MakeRotTransition(JsonConvert.DeserializeObject<Vector3>(m.GetString(0)), 
                        m.GetFloat(1));
                    break;
                #endregion

                #region Selection Room
                case "AcceptRoomJoinRequest":
                    if (m.GetBoolean(0))
                        JoinRoomAsAdmin(m.GetString(1));
                    else
                        JoinRoomAsUser(m.GetString(1));
                    break;
                case "RefuseRoomJoinRequest":
                    RefuseRoomJoin(m.GetString(0));
                    break;
                case "AcceptRoomCreationRequest":
                    CreateMyNewRoom(m.GetString(0));
                    break;
                case "RefuseRoomCreationRequest":
                    RefuseMyRoomCreation();
                    break;
                case "NewRoomAdded":
                    CreateNewRoom(m.GetString(0));
                    break;
                case "AcceptPasswordModificationRequest":
                    FeedbackManager.Instance.CreateFeedBack("Room password changed", FeedbackType.SUCCESS);
                    break;
                case "RefusePasswordModificationRequest":
                    FeedbackManager.Instance.CreateFeedBack("Password modification denied: " + m.GetString(0)
                        , FeedbackType.ERROR);
                    break;
                case "RoomAvailabilityChanged":
                    FeedbackManager.Instance.CreateFeedBack("Room availability successfully changed", 
                        FeedbackType.SUCCESS);
                    break;
                case "RoomAvailabilityNotChanged":
                    FeedbackManager.Instance.CreateFeedBack("Room availability not changed" + m.GetString(0)
                        , FeedbackType.ERROR);
                    break;
                case "RequestRoomInfo":
                    _roomInterfaceController.UpdateRoomInfos(m.GetBoolean(0));
                    break;
                #endregion
            }
        }
        
        _msgList.Clear();
    }
    
    // Handle server messages
    public void HandleMessage(object sender, Message m) {
        _msgList.Add(m);
    }
    
    private void ConnectToRoom(string roomName, string roomType)
    {
        PlayerIO.Authenticate(
            "avatar-controller-toolkit-8g1b8uczeowj6ffvawksg",            //ID provided by the Numediart's Player.io account
            "public",                               //Your connection id
            new Dictionary<string, string> {        //Authentication arguments
                { "userId", UserId },
            },
            null,                                   //PlayerInsight segments
            delegate (Client client) {
                Debug.Log("Successfully connected to Player.IO");

                Debug.Log("Create ServerEndpoint");
                // Comment out the line below to use the live servers instead of your development server
                switch (_networkMode)
                {
                    case NetworkMode.LOCAL:
                        client.Multiplayer.DevelopmentServer = new ServerEndpoint("localhost", 8184);
                        break;
                    case NetworkMode.ONLINE:
                        break;
                    case NetworkMode.OFFLINE:
                        throw new Exception("Network mode set to offline");
                }

                Debug.Log("CreateJoinRoom");
                //Create or join the room 
                client.Multiplayer.CreateJoinRoom(
                    roomName,                    //Room id. If set to null a random roomid is used
                    roomType,                   //The room type started on the server
                    true,                               //Should the room be visible in the lobby?
                    null,
                    null,
                    delegate (Connection connection) {
                        // We successfully joined a room so set up the message handler
                        if (roomType == _roomSelectionType)
                        {
                            _pioconnectionRoom = connection;
                            _pioconnectionRoom.OnMessage += HandleMessage;
                        }
                        else if (roomType == _wozRoomType)
                        {
                            _pioconnectionWoz = connection;
                            _pioconnectionWoz.OnMessage += HandleMessage;
                        }
                        else
                        {
                            throw new Exception("Unknown room type: " + roomType);
                        }
                        
                        FeedbackManager.Instance.CreateFeedBack("Successfully joined room " + roomName, FeedbackType.SUCCESS);
                    },
                    delegate (PlayerIOError error) {
                        Debug.Log("Error Joining Room: " + error.ToString());
                    }
                );
            },
            delegate (PlayerIOError error) {
                Debug.Log("Error connecting: " + error.ToString());
            }
        );
    }

    #region Server Functions called externally

    public void RequestToJoinRoom(string roomId, string roomPassword, bool joinAsAdmin)
    {
        _pioconnectionRoom.Send("RoomJoinRequest", roomId, roomPassword, joinAsAdmin);
    }

    public void RequestToCreateRoom(string roomId, string roomPassword)
    {
        _pioconnectionRoom.Send("RoomCreationRequest", roomId, roomPassword);
    }
    
    public void UserSelectedView(bool isAdminView)
    {
        _pioconnectionWoz.Send("ChoosedView", isAdminView);
    }
    
    public void AvatarHeadMoved(double poseRx, double poseRy, double poseRz)
    {
        if (!IsAdmin)
        {
            throw new Exception("Non-admin could send avatar key move");
        }

        _pioconnectionWoz.Send("AvatarHeadMove", poseRx, poseRy, poseRz);
    }

    public void AvatarBlendShapesMoved(string dictOfBlendShapes)
    {
        if (!IsAdmin)
        {
            throw new Exception("Non-admin could send avatar key move");
        }

        _pioconnectionWoz.Send("AvatarBlendShapesMove", dictOfBlendShapes);
    }

    public void AvatarBlendShapeTransitionToNewFrame(string changesDict, float durationInSeconds)
    {
        if (!IsAdmin)
        {
            throw new Exception("Non-admin could send avatar transition");
        }
        
        _pioconnectionWoz.Send("AvatarBlendShapesTransition", changesDict, durationInSeconds);
    }

    public void AvatarPoseTransitionToNewFrame(string newRot, float durationInSeconds)
    {
        if (!IsAdmin)
        {
            throw new Exception("Non-admin could send avatar transition");
        }
        _pioconnectionWoz.Send("AvatarPoseTransition", newRot, durationInSeconds);
    }
    
    public void DisconnectFromRoomEvent()
    {
        _pioconnectionWoz.Disconnect();
        
        _pioconnectionRoom.Send("PlayerDisconnected", CurrentRoomId, IsAdmin);
        
        _menuManager.LoadRoomSelectionScene();

        // Erase user infos
        IsAdmin = false;
        CurrentRoomId = "";
    }

    public void RequestRoomPasswordModification(string oldPassword, string newPassword)
    {
        if (!IsAdmin)
        {
            throw new Exception("Non-Admin could send password modif request");
        }
        
        _pioconnectionRoom.Send("PasswordModificationRequest", CurrentRoomId, oldPassword, newPassword);
    }

    public void ChangeRoomAvailabilityEvent(bool isRoomAvailable)
    {
        if (!IsAdmin)
        {
            throw new Exception("Non-Admin could change room availability");
        }
        
        _pioconnectionRoom.Send("ChangeRoomAvailability", CurrentRoomId, isRoomAvailable);   
    }

    public void RequestRoomInfos()
    {
        _pioconnectionRoom.Send("RequestRoomInfo", CurrentRoomId);
    }

    public void ReturnToMainMenuEvent()
    {
        if (_pioconnectionWoz != null)
            _pioconnectionWoz.Disconnect();
        
        if (_pioconnectionRoom != null)
            _pioconnectionRoom.Disconnect();
    }
    
    public void ConnectToRoomSelection()
    {
        ConnectToRoom(_mainRoomId, _roomSelectionType);
    }

    #endregion

    #region Room

    // Join
    private void JoinRoomAsAdmin(string roomName)
    {
        _pioconnectionRoom.Send("AdminJoinedRoom", roomName);

        ConnectToRoom(roomName, _wozRoomType);
        
        // Update user infos
        IsAdmin = true;
        CurrentRoomId = roomName;
        
        _roomManager.ResetRoomsDict();
        
        _menuManager.LoadAdminScene();
    }

    private void JoinRoomAsUser(string roomName)
    {
        _pioconnectionRoom.Send("UserJoinedRoom", roomName);

        ConnectToRoom(roomName, _wozRoomType);
        
        // Update user infos
        IsAdmin = false;
        CurrentRoomId = roomName;

        _roomManager.ResetRoomsDict();
        
        _menuManager.LoadUserScene();
    }

    private void RefuseRoomJoin(string refuseMessage)
    {
        FeedbackManager.Instance.CreateFeedBack("Refused room join: " + refuseMessage, FeedbackType.ERROR);
    }

    // Creation
    private void CreateMyNewRoom(string roomName)
    {
        _roomManager.CreateNewRoom(roomName);
        _roomManager.HideRoomCreationMenu();
        
        FeedbackManager.Instance.CreateFeedBack("Room successfully created", FeedbackType.SUCCESS);
    }
    
    private void RefuseMyRoomCreation()
    {
        FeedbackManager.Instance.CreateFeedBack("Room couldn't be added", FeedbackType.ERROR);
    }

    private void CreateNewRoom(string roomName)
    {
        _roomManager.CreateNewRoom(roomName);
        FeedbackManager.Instance.CreateFeedBack("New room added: " + roomName, FeedbackType.INFORMATION);
    }
    
    #endregion
}
