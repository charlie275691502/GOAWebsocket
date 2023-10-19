using System.Collections;
using System.Collections.Generic;
using Common;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace Metagame
{
	public class RoomViewData
	{
		public int Id;
		public string RoomName;
		public GameType GameType;
		public List<PlayerData> Players;
	}
}