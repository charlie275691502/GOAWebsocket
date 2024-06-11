using Data.Sheet;
using Optional.Collections;
using System.Collections.Generic;
using System.Linq;
using Web;
using PlayerViewData = Metagame.PlayerViewData;

namespace Gameplay.GOA
{
	public record CardDataState()
	{
		public record Empty() : CardDataState;
		public record Covered() : CardDataState;
		public record Open(int Key) : CardDataState;
	}
	
	// Must follow backend definition
	public enum Phase
	{
		ChooseBoardCardPhase = 1,
		ActionPhase = 2,
	}
	
	public record GOABoardData(
		int DrawCardCount,
		int GraveCardCount,
		CardDataState[] BoardCards,
		int[] RevealingBoardCardPositions,
		int Turn,
		int TakingTurnPlayerId,
		Phase Phase)
	{
		public GOABoardData(GOABoardResult result) : this(
			result.DrawCardCount,
			result.GraveCardCount,
			result.BoardCards
				.Select<int, CardDataState>(card => card switch
				{
					GOACardUtility.EMPTY_CARD_NUMBER => new CardDataState.Empty(),
					GOACardUtility.COVERED_CARD_NUMBER => new CardDataState.Covered(),
					_ => new CardDataState.Open(card),
				})
				.ToArray(),
			result.RevealingBoardCardPositions,
			result.Turn,
			result.TakingTurnPlayerId,
			(Phase)result.Phase) { }
	}

	public record GOAPlayerData(
		int Order,
		bool IsBot,
		string CharacterKey,
		int[] PublicCards,
		int PublicCardCount,
		int[] StrategyCards,
		int StrategyCardCount,
		int Power,
		int PowerLimit,
		PlayerViewData Player,
		int Elo,
		int PlayedGameCount,
		int WinGameCount)
	{
		public GOAPlayerData(GOAPlayerResult result, IGoogleSheetLoader googleSheetLoader) : this(
			result.Order,
			result.IsBot,
			result.CharacterKey,
			result.PublicCards,
			result.PublicCardCount,
			result.StrategyCards,
			result.StrategyCardCount,
			result.Power,
			result.PowerLimit,
			new PlayerViewData(result.Player, googleSheetLoader),
			result.Elo,
			result.PlayedGameCount,
			result.WinGameCount) { }
	}

	public record GOASettingData()
	{
		public GOASettingData(GOASettingResult result) : this(
			) { }
	}

	public record GOAGameData(
		int GameId,
		int SelfPlayerId,
		GOABoardData Board,
		GOAPlayerData[] Players,
		GOASettingData Setting) : IGameData
	{
		public GOAGameData(GOAGameResult result, int selfPlayerId, IGoogleSheetLoader googleSheetLoader) : this(
			result.Id,
			selfPlayerId,
			new GOABoardData(result.Board),
			_GetGOAPlayerDatas(result.Players, googleSheetLoader),
			new GOASettingData(result.Setting)) { }

		public static GOAPlayerData[] _GetGOAPlayerDatas(GOAPlayerResult[] players, IGoogleSheetLoader googleSheetLoader)
			=> players
				.Select(player => new GOAPlayerData(player, googleSheetLoader))
				.ToArray();
	}
}