using System.Collections.Generic;
using Newtonsoft.Json;

namespace Web
{
	public class LoginResult
	{
		[JsonProperty("access")]
		public string AccessKey;
		[JsonProperty("refresh")]
		public string RefreshKey;
	}
	
	public class PlayerDataResult
	{
		[JsonProperty("id")]
		public int Id;
		[JsonProperty("nick_name")]
		public string NickName;
		[JsonProperty("coin")]
		public int Coin;
		[JsonProperty("avatar_key")]
		public string AvatarKey;
	}
	
	public class GameSettingResult
	{
		[JsonProperty("id")]
		public int Id;
		[JsonProperty("game_type")]
		public string GameType;
		[JsonProperty("player_plot")]
		public int PlayerPlot;
	}
	
	public class RoomResult
	{
		[JsonProperty("id")]
		public int Id;
		[JsonProperty("room_name")]
		public string RoomName;
		[JsonProperty("game_setting")]
		public GameSettingResult GameSetting;
		[JsonProperty("players")]
		public List<PlayerDataResult> Players;
	}
	
	public class RoomListResult : List<RoomResult>
	{
		
	}
	
	public class MessagePlayerResult
	{
		[JsonProperty("id")]
		public int Id;
		[JsonProperty("nick_name")]
		public string NickName;
		[JsonProperty("avatar_key")]
		public string AvatarKey;
	}
	
	public class MessageResult
	{
		[JsonProperty("id")]
		public int Id;
		[JsonProperty("content")]
		public string Content;
		[JsonProperty("player")]
		public MessagePlayerResult Player;
	}
	
	public class RoomWithMessagesResult : RoomResult
	{
		[JsonProperty("messages")]
		public List<MessageResult> Messages;
	}
	
#region TTT

	public class TicTacToeGameResult
	{
		[JsonProperty("id")]
		public int Id;
		[JsonProperty("board")]
		public TicTacToeBoardResult Board;
		[JsonProperty("players")]
		public TicTacToePlayerResult[] Players;
		[JsonProperty("setting")]
		public TicTacToeSettingResult Setting;
	}

	public class TicTacToeSummaryResult
	{
		[JsonProperty("winner")]
		public TicTacToePlayerResult Winner;
		[JsonProperty("turns")]
		public int Turns;
	}

	public class TicTacToeRecordResult
	{
		[JsonProperty("id")]
		public int Id;
		[JsonProperty("init_board")]
		public TicTacToeBoardResult InitBoard;
		[JsonProperty("players")]
		public TicTacToePlayerResult[] Players;
		[JsonProperty("actions")]
		public TicTacToeActionResult[] Actions;
		[JsonProperty("setting")]
		public TicTacToeSettingResult Setting;
		[JsonProperty("summary")]
		public TicTacToeSummaryResult Summary;
	}

	public class TicTacToeBoardResult
	{
		[JsonProperty("positions")]
		public int[] Positions;
		[JsonProperty("turn")]
		public int Turn;
		[JsonProperty("turn_of_team")]
		public int TurnOfTeam;
	}

	public class TicTacToeSettingResult
	{
		[JsonProperty("board_size")]
		public int BoardSize;
	}

	public class TicTacToePlayerResult
	{
		[JsonProperty("team")]
		public int Team;
		[JsonProperty("player")]
		public PlayerDataResult Player;
		[JsonProperty("elo")]
		public int Elo;
		[JsonProperty("played_game_count")]
		public int PlayedGameCount;
		[JsonProperty("win_game_count")]
		public int WinGameCount;
	}

	public class TicTacToeActionResult
	{
		[JsonProperty("player_id")]
		public int PlayerId;
		[JsonProperty("action_command_type")]
		public string ActionCommandType;
		[JsonProperty("action_command")]
		public TicTacToeActionCommandResult ActionCommand;
	}

	public class TicTacToeActionCommandResult
	{

	}

	public class TicTacToeChoosePositionActionCommandResult : TicTacToeActionCommandResult
	{
		[JsonProperty("position")]
		public int Position;
		[JsonProperty("value")]
		public int Value;
	}

	public class TicTacToeResignActionCommandResult : TicTacToeActionCommandResult
	{

	}
	
#endregion

#region GOA

	public class GOAGameResult
	{
		[JsonProperty("id")]
		public int Id;
		[JsonProperty("board")]
		public GOABoardResult Board;
		[JsonProperty("players")]
		public GOAPlayerResult[] Players;
		[JsonProperty("setting")]
		public GOASettingResult Setting;
	}

	public class GOASummaryResult
	{
		[JsonProperty("winner")]
		public GOAPlayerResult Winner;
		[JsonProperty("turns")]
		public int Turns;
	}

	public class GOARecordResult
	{
		[JsonProperty("id")]
		public int Id;
		[JsonProperty("init_board")]
		public GOABoardResult InitBoard;
		[JsonProperty("players")]
		public GOAPlayerResult[] Players;
		[JsonProperty("actions")]
		public GOAActionResult[] Actions;
		[JsonProperty("setting")]
		public GOASettingResult Setting;
		[JsonProperty("summary")]
		public TicTacToeSummaryResult Summary;
	}

	public class GOABoardResult
	{
		[JsonProperty("draw_card_count")]
		public int DrawCardCount;
		[JsonProperty("grave_card_count")]
		public int GraveCardCount;
		[JsonProperty("cards")]
		public int[] Cards;
		[JsonProperty("turn")]
		public int Turn;
		[JsonProperty("taking_turn_player_id")]
		public int TakingTurnPlayerId;
	}

	public class GOASettingResult
	{
		
	}

	public class GOAPlayerResult
	{
		[JsonProperty("order")]
		public int Order;
		[JsonProperty("is_bot")]
		public bool IsBot;
		[JsonProperty("character_key")]
		public string CharacterKey;
		[JsonProperty("public_cards")]
		public int[] PublicCards;
		[JsonProperty("public_card_count")]
		public int PublicCardCount;
		[JsonProperty("strategy_cards")]
		public int[] StrategyCards;
		[JsonProperty("strategy_card_count")]
		public int StrategyCardCount;
		[JsonProperty("power")]
		public int Power;
		[JsonProperty("power_limit")]
		public int PowerLimit;
		[JsonProperty("player")]
		public PlayerDataResult Player;
		[JsonProperty("elo")]
		public int Elo;
		[JsonProperty("played_game_count")]
		public int PlayedGameCount;
		[JsonProperty("win_game_count")]
		public int WinGameCount;
	}

	public class GOAActionResult
	{
		[JsonProperty("player_id")]
		public int PlayerId;
		[JsonProperty("action_command_type")]
		public string ActionCommandType;
		[JsonProperty("action_command")]
		public GOAActionCommandResult ActionCommand;
	}

	public class GOAActionCommandResult
	{

	}

	public class GOARevealBoardCardsActionCommandResult : GOAActionCommandResult
	{
		[JsonProperty("positions")]
		public int[] Positions;
	}

	public class GOAChooseBoardCardActionCommandResult : GOAActionCommandResult
	{
		[JsonProperty("position")]
		public int Position;
	}
}

#endregion