using Common;
using Metagame.MainPage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metagame.Room
{
	public class MessageViewData
	{
		public int Id;
		public string Content;
		public string NickName;
	}
	
	public record RoomWithMessagesViewData(
		int Id,
		string RoomName,
		GameSetting GameSetting,
		List<PlayerData> Players,
		List<MessageViewData> Messages) 
			: RoomViewData(
				Id,
				RoomName,
				GameSetting,
				Players);
}