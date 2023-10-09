using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("Instances")] [SerializeField] private GameObject _roomPrefab;
    [SerializeField] private Transform _roomParent;
    [SerializeField] private GameObject _roomCreationGo;
    
    [Header("Parameters")] [SerializeField] private KeyCode _keyToToggleRoomCreationMenu;

    private Dictionary<string, Room> _roomsByName = new Dictionary<string, Room>();

    private void Update()
    {
        if (Input.GetKeyDown(_keyToToggleRoomCreationMenu))
        {
            ToggleRoomCreationMenu();
        }
    }

    public void CreateNewRoom(string roomName)
    {
        GameObject go = Instantiate(_roomPrefab, _roomParent);
        
        Room newRoom = go.GetComponent<Room>();
        newRoom.Id = roomName;
        _roomsByName.Add(roomName, newRoom);
        
        newRoom.PersonalizeRoomPrefab();
    }

    public void DeleteRoom(string roomName)
    {
        if (!_roomsByName.ContainsKey(roomName))
            return;
        
        // Destroy gameobject
        Destroy(_roomsByName[roomName].gameObject);
        
        // Remove reference
        _roomsByName.Remove(roomName);
    }

    public void ToggleRoomCreationMenu()
    {
        _roomCreationGo.SetActive(!_roomCreationGo.activeSelf);
    }

    public void HideRoomCreationMenu()
    {
        _roomCreationGo.SetActive(false);
    }

    public void ResetRoomsDict()
    {
        _roomsByName.Clear();
    }
}
