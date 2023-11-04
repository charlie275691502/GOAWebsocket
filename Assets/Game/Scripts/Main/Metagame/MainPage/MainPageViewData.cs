using System.Collections.Generic;
using Common;

namespace Metagame.MainPage
{
	public record RoomViewData(int Id, string RoomName, GameSetting GameSetting, List<PlayerData> Players);
}