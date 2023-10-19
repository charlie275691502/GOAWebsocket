using System.Collections.Generic;
using System.Linq;
using TMPro;
using Optional;
using Optional.Collections;
using Optional.Unsafe;

namespace Metagame
{
	public enum GameType
	{
		Tic_Tac_Toe,
		Generation_Of_Authority,
	};
	
	public static class GameTypeUtility
	{
		private static Dictionary<GameType, string> _dic = new Dictionary<GameType, string>()
		{
			{GameType.Tic_Tac_Toe, "TTT"},
			{GameType.Generation_Of_Authority, "GOA"},
		};
		
		public static GameType GetGameType(string abr)
			=> _dic
				.FirstOrNone(pair => pair.Value == abr)
				.Map(pair => pair.Key)
				.ValueOrFailure($"[{abr}] is not defined in GameTypeUtility._dic");
			
		public static string GetAbbreviation(GameType gameType)
			=> _dic
				.GetValueOrNone(gameType)
				.ValueOr($"[{gameType}] is not defined in GameTypeUtility._dic");
	}
}