using System.Collections;
using System.Collections.Generic;
using Common;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace Metagame.MainPage
{
	public record RoomViewData(int Id, string RoomName, GameSetting GameSetting, List<PlayerData> Players);
}