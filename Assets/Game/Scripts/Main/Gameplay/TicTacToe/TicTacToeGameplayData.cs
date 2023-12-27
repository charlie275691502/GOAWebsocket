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
		int WinGameCount)
	{
		public TicTacToePlayerData(TicTacToePlayerResult result) : this(
			result.Team,
			new PlayerViewData(result.Player),
			result.Elo,
			result.PlayedGameCount,
			result.WinGameCount) { }
	}

	public record TicTacToeSettingData(
		int BoardSize)
	{
		public TicTacToeSettingData(TicTacToeSettingResult result) : this(
			result.BoardSize) { }
	}

	public record TicTacToeGameData(
		TicTacToeBoardData Board,
		TicTacToePlayerData[] Players,
		TicTacToeSettingData Setting) : IGameData
	{
		public TicTacToeGameData(TicTacToeGameResult result) : this(
			new TicTacToeBoardData(result.Board),
			result.Players
				.Select(player => new TicTacToePlayerData(player))
				.ToArray(),
			new TicTacToeSettingData(result.Setting)) { }
    }
}