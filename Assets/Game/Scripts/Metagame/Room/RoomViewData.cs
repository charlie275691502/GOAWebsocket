using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metagame
{
	public class MessageViewData
	{
		public int Id;
		public string Content;
		public string NickName;
	}
	
	public class RoomWithMessagesViewData : RoomViewData
	{
		public List<MessageViewData> Messages;
	}
}