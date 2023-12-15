using Common;
using Metagame.MainPage;
using System.Collections.Generic;

namespace Metagame.Room
{
	public class MessageViewData
	{
		public int Id;
		public string Content;
		public string NickName;
		public string AvatarImageKey;
	}
	
	public record RoomWithMessagesViewData(
		int Id,
		string RoomName,
		GameSetting GameSetting,
		List<PlayerViewData> Players,
		List<MessageViewData> Messages,
		bool EnableStartGameButton) 
			: RoomViewData(
				Id,
				RoomName,
				GameSetting,
				Players);
}