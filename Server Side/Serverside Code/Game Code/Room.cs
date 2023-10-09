using System;
using System.Collections.Generic;
using PlayerIO.GameLibrary;

namespace Avatar_Controller_Toolkit
{
    public class Room
    {
        public string Id;
        public string Password;
        public bool IsOpen;
        public bool ContainsAdmin;
        public List<User> UsersConnected = new List<User>();
    }

    [RoomType("Room_Selection")]
    public class RoomCode : Game<User>
    {
        public Dictionary<string, Room> RoomsByName = new Dictionary<string, Room>();

        public override void GameStarted()
        {
            Console.WriteLine("Room_Selection opened " + RoomId);
        }

        // This method is called when the last player leaves the room, and it's closed down.
        public override void GameClosed()
        {
            Console.WriteLine("Room_Selection closed: " + RoomId);
        }

        // This method is called whenever a player joins the game
        // It needs to send to each player every rooms created
        public override void UserJoined(User user)
        {
            // Send all rooms to new User
            foreach (var pair in RoomsByName)
            {
                user.Send("NewRoomAdded", pair.Key);
            }

            // Notify all players of new player
            foreach (User us in Players)
            {
                if (us.ConnectUserId != user.ConnectUserId)
                {
                    us.Send("UserConnected", user.ConnectUserId);
                    user.Send("UserConnected", us.ConnectUserId);
                }
            }
        }

        // This method is called when a player leaves the game
        public override void UserLeft(User user)
        {
            Console.WriteLine("User disconnected or joined a room");

            Broadcast("UserLeft", user.ConnectUserId);
        }

        public override void GotMessage(User user, Message message)
        {
            switch (message.Type)
            {     
                case "RoomJoinRequest":
                    // Handle request
                    string roomName = message.GetString(0);
                    string password = message.GetString(1);
                    bool adminRequested = message.GetBoolean(2);

                    if (RoomsByName[roomName].Password != password)
                    {
                        user.Send("RefuseRoomJoinRequest", "Wrong password");
                        break;
                    }
                    
                    if (!RoomsByName[roomName].IsOpen)
                    {
                        if (!adminRequested)
                            user.Send("RefuseRoomJoinRequest", "Room not opened");
                        else if (RoomsByName[roomName].ContainsAdmin)
                            user.Send("RefuseRoomJoinRequest", "Room contains admin and is not opened");
                        else
                            user.Send("AcceptRoomJoinRequest", adminRequested, roomName);
                        break;
                    }

                    // Partially accepted
                    if (adminRequested && RoomsByName[roomName].ContainsAdmin)
                    {
                        user.Send("AcceptRoomJoinRequest", !adminRequested, roomName);
                        break;
                    }

                    // Full acceptation 
                    user.Send("AcceptRoomJoinRequest", adminRequested, roomName);
                    break;
                case "AdminJoinedRoom":
                    RoomsByName[message.GetString(0)].ContainsAdmin = true;
                    user.IsAdmin = true;
                    AddUserToRoom(user, message.GetString(0));
                    break;
                case "UserJoinedRoom":
                    user.IsAdmin = false;
                    AddUserToRoom(user, message.GetString(0));
                    break;
                case "RoomCreationRequest":
                    // Handle Request
                    string newRoomName = message.GetString(0);
                    string newRoomPassword = message.GetString(1);

                    if (RoomsByName.ContainsKey(newRoomName))
                    {
                        user.Send("RefuseRoomCreationRequest");
                        break;
                    }

                    CreateRoom(newRoomName, newRoomPassword);

                    foreach(User us in Players)
                    {
                        if (us.ConnectUserId != user.ConnectUserId)
                        {
                            us.Send("NewRoomAdded", newRoomName);
                        }
                        else
                        {
                            user.Send("AcceptRoomCreationRequest", newRoomName);
                        }
                    }
                    break;
                case "PlayerDisconnected":
                    RemoveUserFromRoom(user, message.GetString(0), message.GetBoolean(1));
                    // Send all rooms to User that is back in the Room Selection Scene
                    foreach (var pair in RoomsByName)
                    {
                        user.Send("NewRoomAdded", pair.Key);
                    }
                    break;
                case "PasswordModificationRequest":
                    // Handle Request
                    string roomNameForPasswordModif = message.GetString(0);
                    string oldPassword = message.GetString(1);
                    string newPassword = message.GetString(2);

                    if (!RoomsByName.ContainsKey(roomNameForPasswordModif))
                    {
                        user.Send("RefusePasswordModificationRequest", "Room doesn't exist");
                        break;
                    }

                    if (RoomsByName[roomNameForPasswordModif].Password != oldPassword)
                    {
                        user.Send("RefusePasswordModificationRequest", "Old password incorrect");
                        break;
                    }

                    RoomsByName[roomNameForPasswordModif].Password = newPassword;
                    user.Send("AcceptPasswordModificationRequest");
                    break;
                case "ChangeRoomAvailability":
                    string roomNameForAvailabilityChange = message.GetString(0);

                    if (RoomsByName.TryGetValue(roomNameForAvailabilityChange, out var room))
                    {
                        ChangeRoomAvailability(room, message.GetBoolean(1));
                        user.Send("RoomAvailabilityChanged", roomNameForAvailabilityChange);
                        break;
                    }

                    user.Send("RoomAvailabilityNotChanged", "Room doesn't exist");
                    break;
                case "RequestRoomInfo":
                    user.Send("RequestRoomInfo", RoomsByName[message.GetString(0)].IsOpen);
                    break;
            }
        }

        private void CreateRoom(string name, string password)
        {
            if (RoomsByName.ContainsKey(name))
                return;

            Room newRoom = new Room();
            newRoom.Id = name;
            newRoom.IsOpen = true;
            newRoom.Password = password;
            newRoom.ContainsAdmin = false;
            newRoom.UsersConnected = new List<User>();

            RoomsByName.Add(newRoom.Id, newRoom);
        }

        private void ChangeRoomAvailability(Room room, bool isAvailable)
        {
            room.IsOpen = isAvailable;
        }

        private void AddUserToRoom(User user, string roomName)
        {
            if (!RoomsByName.ContainsKey(roomName))
            {
                return;
            }

            RoomsByName[roomName].UsersConnected.Add(user);
        }

        private void RemoveUserFromRoom(User user, string roomName, bool isAdmin)
        {
            if (!RoomsByName.ContainsKey(roomName))
                return;

            RoomsByName[roomName].UsersConnected.Remove(user);

            if (isAdmin)
                RoomsByName[roomName].ContainsAdmin = false;
        }
    }
}
