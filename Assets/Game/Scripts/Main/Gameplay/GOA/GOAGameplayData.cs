using Optional.Collections;
using System.Linq;
using Web;
using PlayerViewData = Metagame.PlayerViewData;

namespace Gameplay.GOA
{
	public record GOABoardData(
		int[] Positions,
		int Turn,
		int TurnOfTeam)
    {
		public GOABoardData(GOABoardResult result) : this(
			result.Positions,
			result.Turn,
			result.TurnOfTeam) { }
	}

	public record GOAPlayerData(
		int Team,
        PlayerViewData Player,
		int Elo,
		int PlayedGameCount,
		int WinGameCount,
		bool IsSelfPlayer,
		bool IsSelfTeam)
	{
		public GOAPlayerData(GOAPlayerResult result, bool isSelfPlayer, bool isSelfTeam) : this(
			result.Team,
			new PlayerViewData(result.Player),
			result.Elo,
			result.PlayedGameCount,
			result.WinGameCount,
			isSelfPlayer,
			isSelfTeam) { }
	}

	public record GOASettingData(
		int BoardSize)
	{
		public GOASettingData(GOASettingResult result) : this(
			result.BoardSize) { }
	}

	public record GOAGameData(
		int GameId,
		int SelfPlayerId,
		int SelfPlayerTeam,
		GOABoardData Board,
		GOAPlayerData[] Players,
		GOASettingData Setting) : IGameData
	{
		public GOAGameData(GOAGameResult result, int selfPlayerId) : this(
			result.Id,
			selfPlayerId,
			_GetSelfPlayerTeam(result.Players, selfPlayerId),
			new GOABoardData(result.Board),
			_GetGOAPlayerDatas(result.Players, selfPlayerId),
			new GOASettingData(result.Setting)) { }

		public static GOAPlayerData[] _GetGOAPlayerDatas(GOAPlayerResult[] players, int selfPlayerId)
        {
			var selfPlayerTeam = _GetSelfPlayerTeam(players, selfPlayerId);
			return players
				.Select(player => new GOAPlayerData(
					player,
					selfPlayerId == player.Player.Id,
					selfPlayerTeam == player.Team))
				.ToArray();
		}

		public static int _GetSelfPlayerTeam(GOAPlayerResult[] players, int selfPlayerId)
			=> players
				.FirstOrNone(player => player.Player.Id == selfPlayerId)
				.Map(player => player.Team)
				.ValueOr(0);
	}
}