using Data.Sheet;
using Optional.Collections;
using System.Linq;
using Web;
using PlayerViewData = Metagame.PlayerViewData;

namespace Gameplay.TicTacToe
{
	public record TicTacToeBoardData(
		int[] Positions,
		int Turn,
		int TurnOfTeam)
	{
		public TicTacToeBoardData(TicTacToeBoardResult result) : this(
			result.Positions,
			result.Turn,
			result.TurnOfTeam) { }
	}

	public record TicTacToePlayerData(
		int Team,
		PlayerViewData Player,
		int Elo,
		int PlayedGameCount,
		int WinGameCount,
		bool IsSelfPlayer,
		bool IsSelfTeam)
	{
		public TicTacToePlayerData(TicTacToePlayerResult result, bool isSelfPlayer, bool isSelfTeam, IGoogleSheetLoader googleSheetLoader) : this(
			result.Team,
			new PlayerViewData(result.Player, googleSheetLoader),
			result.Elo,
			result.PlayedGameCount,
			result.WinGameCount,
			isSelfPlayer,
			isSelfTeam) { }
	}

	public record TicTacToeSettingData(
		int BoardSize)
	{
		public TicTacToeSettingData(TicTacToeSettingResult result) : this(
			result.BoardSize) { }
	}

	public record TicTacToeGameData(
		int GameId,
		int SelfPlayerId,
		int SelfPlayerTeam,
		TicTacToeBoardData Board,
		TicTacToePlayerData[] Players,
		TicTacToeSettingData Setting) : IGameData
	{
		public TicTacToeGameData(TicTacToeGameResult result, int selfPlayerId, IGoogleSheetLoader googleSheetLoader) : this(
			result.Id,
			selfPlayerId,
			_GetSelfPlayerTeam(result.Players, selfPlayerId),
			new TicTacToeBoardData(result.Board),
			_GetTicTacToePlayerDatas(result.Players, selfPlayerId, googleSheetLoader),
			new TicTacToeSettingData(result.Setting)) { }

		public static TicTacToePlayerData[] _GetTicTacToePlayerDatas(TicTacToePlayerResult[] players, int selfPlayerId, IGoogleSheetLoader googleSheetLoader)
		{
			var selfPlayerTeam = _GetSelfPlayerTeam(players, selfPlayerId);
			return players
				.Select(player => new TicTacToePlayerData(
					player,
					selfPlayerId == player.Player.Id,
					selfPlayerTeam == player.Team,
					googleSheetLoader))
				.ToArray();
		}

		public static int _GetSelfPlayerTeam(TicTacToePlayerResult[] players, int selfPlayerId)
			=> players
				.FirstOrNone(player => player.Player.Id == selfPlayerId)
				.Map(player => player.Team)
				.ValueOr(0);
	}
}