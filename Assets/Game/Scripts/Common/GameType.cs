using System.Collections.Generic;
using Optional.Collections;
using Optional.Unsafe;
using Web;

namespace Common.Class
{
	public enum GameType
	{
		None,
		TicTacToe,
		GenerationOfAuthority,
	};
	
	public class GameSetting
	{
		public GameType GameType;
		public int PlayerPlot;
		
		public GameSetting(GameSettingResult result)
		{
			GameType = GameTypeUtility.GetGameType(result.GameType);
			PlayerPlot = result.PlayerPlot;
		}
	}
	
	public static class GameTypeUtility
	{
		private static Dictionary<GameType, string> _dic = new Dictionary<GameType, string>()
		{
			{GameType.None, "NON"},
			{GameType.TicTacToe, "TTT"},
			{GameType.GenerationOfAuthority, "GOA"},
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