using System;
using System.Collections.Generic;
using PlayerIO.GameLibrary;

namespace Avatar_Controller_Toolkit
{
	public class User : BasePlayer
	{
		public bool IsAdmin;
	}


	[RoomType("Room_Wizard_Of_Oz")]
	public class GameCode : Game<User>
	{

		// This method is called when an instance of your the game is created
		public override void GameStarted()
		{
			// anything you write to the Console will show up in the 
			// output window of the development server
			Console.WriteLine("Game is started: " + RoomId);
		}

		// This method is called when the last player leaves the room, and it's closed down.
		public override void GameClosed()
		{
			Console.WriteLine("RoomId: " + RoomId);
		}

		// This method is called whenever a player joins the game
		public override void UserJoined(User user)
		{
			// Ask about the <User> instance
			user.Send("AskForUserInformation");

			// Notify players of join
			foreach (User pl in Players)
			{
				if (pl.ConnectUserId != user.ConnectUserId)
				{
					pl.Send("UserConnected", user.ConnectUserId);
					user.Send("UserConnected", pl.ConnectUserId);
				}
			}
		}

		// This method is called when a player leaves the game
		public override void UserLeft(User player)
		{
			Broadcast("PlayerLeft", player.ConnectUserId);
		}

		// This method is called when a player sends a message into the server code
		public override void GotMessage(User player, Message message)
		{
			switch (message.Type)
			{
				case "Chat":
					foreach (User pl in Players)
					{
						if (pl.ConnectUserId != player.ConnectUserId)
						{
							pl.Send("Chat", player.ConnectUserId, message.GetString(0));
						}
					}
					break;
				case "SendUserInfos":
					player.IsAdmin = message.GetBoolean(0);
					break;
				case "AvatarHeadMove":
					// check it's an admin that sent the movement
					if (!player.IsAdmin)
						break;

					foreach (User pl in Players)
					{
						if (pl.ConnectUserId != player.ConnectUserId)
						{
							pl.Send("AvatarHeadMove", message.GetDouble(0), message.GetDouble(1), message.GetDouble(2));
						}
					}
					break;
				case "AvatarBlendShapesMove":
					// check it's an admin that sent the movement
					if (!player.IsAdmin)
						break;

					foreach (User pl in Players)
					{
						if (pl.ConnectUserId != player.ConnectUserId)
						{
							pl.Send("AvatarBlendShapesMove", message.GetString(0));
						}
					}
					break;
				case "AvatarBlendShapesTransition":
					// check it's an admin that sent the movement
					if (!player.IsAdmin)
						break;

					foreach (User pl in Players)
					{
						if (pl.ConnectUserId != player.ConnectUserId)
						{
							pl.Send("AvatarBlendShapesTransition", message.GetString(0), message.GetFloat(1));
						}
					}
					break;
				case "AvatarPoseTransition":
					// check it's an admin that sent the movement
					if (!player.IsAdmin)
						break;

					foreach (User pl in Players)
					{
						if (pl.ConnectUserId != player.ConnectUserId)
						{
							pl.Send("AvatarPoseTransition", message.GetString(0), message.GetFloat(1));
						}
					}
					break;
			}
		}
	}
}