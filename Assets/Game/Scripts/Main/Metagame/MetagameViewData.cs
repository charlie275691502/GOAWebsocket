using System.Collections.Generic;
using Common.Class;
using Web;

namespace Metagame
{
	public record RoomViewData(int Id, string RoomName, GameSetting GameSetting, List<PlayerViewData> Players);
	public record PlayerViewData(int Id, string NickName, int Coin, string AvatarImageKey)
	{
		public PlayerViewData(PlayerDataResult playerDataResult) : 
			this(
				playerDataResult.Id,
				playerDataResult.NickName,
				playerDataResult.Coin,
				"Red") { }
	}
}