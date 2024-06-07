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
		public record Open(string Id) : CardDataState;
	}
	
	public record GOABoardData(
		int DrawCardCount,
		int GraveCardCount,
		CardDataState[] Cards,
		int Turn,
		int TakingTurnPlayerId)
	{
		public GOABoardData(GOABoardResult result) : this(
			result.DrawCardCount,
			result.GraveCardCount,
			result.Cards
				.Select<string, CardDataState>(card => card switch
				{
					GOACardUtility.EMPTY_CARD_ID => new CardDataState.Empty(),
					GOACardUtility.COVERED_CARD_ID => new CardDataState.Covered(),
					_ => new CardDataState.Open(card),
				})
				.ToArray(),
			result.Turn,
			result.TakingTurnPlayerId) { }
	}

	public record GOAPlayerData(
		int Order,
		bool IsBot,
		string CharacterId,
		string[] PublicCardIds,
		int PublicCardCount,
		string[] StrategyCardIds,
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
			result.CharacterId,
			result.PublicCardIds,
			result.PublicCardCount,
			result.StrategyCardIds,
			result.StrategyCardCount,
			result.Power,
			result.PowerLimit,
			new PlayerViewData(result.Player, googleSheetLoader),
			result.Elo,
			result.PlayedGameCount,
			result.WinGameCount) { }
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