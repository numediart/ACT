using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomInterfaceController : MonoBehaviour
{
    [Header("Instances")] [SerializeField] private GameObject _roomInterface;
    [SerializeField] private TMP_InputField[] _passwordInputs;
    [SerializeField] private Toggle _toggle;
    
    [Header("Controls")] [SerializeField] private KeyCode _roomInterfaceToggler;

    [Header("Parameters")] [SerializeField] private bool _isAdminSide;

    #region Intern var

    private string _oldPasswordInput;
    private string _newPasswordInput;
    private string _confirmNewPasswordInput;
    private bool _isRoomAvailable;
    
    #endregion

    private void Start()
    {
        if (_isAdminSide)
        {
            NetworkManager.Instance.RequestRoomInfos();
            ResetInputs();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(_roomInterfaceToggler))
            ToggleRoomInterface();
    }

    private void ResetInputs()
    {
        foreach (var input in _passwordInputs)
        {
            input.text = "";
        }
        
        _oldPasswordInput = "";
        _newPasswordInput = "";
        _confirmNewPasswordInput = "";
    }

    public void UpdateRoomInfos(bool isRoomAvailable)
    {
        _isRoomAvailable = isRoomAvailable;
        _toggle.isOn = isRoomAvailable;
    }
    
    public void ToggleRoomInterface()
    {
        _roomInterface.SetActive(!_roomInterface.activeSelf);
    }

    public void DisconnectFromRoom()
    {
        //Call network manager event
        NetworkManager.Instance.DisconnectFromRoomEvent();
    }

    #region Room Modification with server call
    private void ToggleRoomAvailabilityForServer()
    {
        //Call network manager event to modify room
        NetworkManager.Instance.ChangeRoomAvailabilityEvent(_isRoomAvailable);
    }

    public void ModifyRoomPassword()
    {
        if (_newPasswordInput == _confirmNewPasswordInput)
        {
            // Call Network Manager event to modify room
            NetworkManager.Instance.RequestRoomPasswordModification(_oldPasswordInput, _newPasswordInput);
        }
        else
        {
            FeedbackManager.Instance.CreateFeedBack("New passwords don't match", FeedbackType.ERROR);
        }
        
        // Reset Inputs
        ResetInputs();
    }
    #endregion

    #region Dynamic Listeners
    public void UpdateRoomAvailability(bool isAvailable)
    {
        _isRoomAvailable = isAvailable;
        ToggleRoomAvailabilityForServer();
    }
    
    public void UpdatePasswordInput(string input)
    {
        _newPasswordInput = input;
    }

    public void UpdateOldPasswordInput(string input)
    {
        _oldPasswordInput = input;
    }
    
    public void UpdateConfirmPasswordInput(string input)
    {
        _confirmNewPasswordInput = input;
    }
    #endregion
}
