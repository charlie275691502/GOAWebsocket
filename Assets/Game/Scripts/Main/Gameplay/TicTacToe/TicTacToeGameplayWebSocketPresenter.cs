using System;
using System.Collections.Generic;
using Common;
using Common.UniTaskExtension;
using Cysharp.Threading.Tasks;
using OneOf;
using OneOf.Types;
using Web;

namespace Gameplay.TicTacToe
{
	public interface ITicTacToeGameplayWebSocketPresenter : IWebSocketPresenter
	{
		UniTask<OneOf<None, UniTaskError>> Start(int roomId);
		UniTask<OneOf<None, UniTaskError>> ChoosePosition(int position);
		void RegisterOnUpdateGame(Action<TicTacToeGameResult> onReceiveMessage);
		void RegisterOnReceiveGameOver(Action<TicTacToeGameResult> onReceiveMessage);
	}
	
	public class TicTacToeGameplayWebSocketPresenter : WebSocketPresenter, ITicTacToeGameplayWebSocketPresenter
	{
		public TicTacToeGameplayWebSocketPresenter(BackendPlayerData backendPlayerData) : base(backendPlayerData)
		{
		}

		UniTask<OneOf<None, UniTaskError>> ITicTacToeGameplayWebSocketPresenter.Start(int roomId)
		{
			return _StartWebsocket(string.Format("TTTGame/games/{0}/", roomId.ToString()));
		}
		
		UniTask<OneOf<None, UniTaskError>> ITicTacToeGameplayWebSocketPresenter.ChoosePosition(int position)
			=>
				_SendWaitTillReturn<None>(
					new Dictionary<string, object>()
					{
						{"command", "choose_position_action"},
						{"position", position},
					});

		void ITicTacToeGameplayWebSocketPresenter.RegisterOnUpdateGame(Action<TicTacToeGameResult> onReceiveMessage)
		{
			_RegisterOnReceiveMessage("update_game", onReceiveMessage);
		}
		
		void ITicTacToeGameplayWebSocketPresenter.RegisterOnReceiveGameOver(Action<TicTacToeGameResult> onReceiveMessage)
		{
			_RegisterOnReceiveMessage("game_over", onReceiveMessage);
		}
	}
}