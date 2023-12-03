using System.Collections.Generic;
using Data.Sheet;
using Optional.Unsafe;
using Web;

namespace Metagame
{
	public record RoomViewData(int Id, string RoomName, GameSetting GameSetting, List<PlayerViewData> Players);
	public record PlayerViewData(int Id, string NickName, int Coin, string AvatarImageKey)
	{
		public PlayerViewData(PlayerDataResult playerDataResult, IExcelDataSheetLoader excelDataSheetLoader) : 
			this(
				playerDataResult.Id,
				playerDataResult.NickName,
				playerDataResult.Coin,
				excelDataSheetLoader.Container.Avatars
					.GetRow(playerDataResult.AvatarId)
					.Map(avatar => avatar.ImageKey)
					.ValueOr(string.Empty)) { }
	}
}