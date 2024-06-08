using System;
using System.Collections.Generic;
using Common;
using Common.UniTaskExtension;
using Cysharp.Threading.Tasks;
using OneOf;
using OneOf.Types;
using Web;

namespace Metagame
{
	public interface IRoomWebSocketPresenter : IWebSocketPresenter
	{
		UniTask<OneOf<None, UniTaskError>> Start(int roomId);
		void SendMessage(string message);
		UniTask<OneOf<None, UniTaskError>> JoinRoom();
		UniTask<OneOf<None, UniTaskError>> SendStartGame();
		void RegisterOnReceiveAppendMessage(Action<MessageResult> onReceiveMessage);
		void RegisterOnReceiveUpdateRoom(Action<RoomResult> onReceiveMessage);
		void RegisterOnReceiveStartTTTGame(Action<TicTacToeGameResult> onReceiveMessage);
		void RegisterOnReceiveStartGOAGame(Action<GOAGameResult> onReceiveMessage);
	}
	
	public class RoomWebSocketPresenter : WebSocketPresenter, IRoomWebSocketPresenter
	{
		public RoomWebSocketPresenter(ISetting setting, BackendPlayerData backendPlayerData) : base(setting, backendPlayerData)
		{
		}

		UniTask<OneOf<None, UniTaskError>> IRoomWebSocketPresenter.Start(int roomId)
		{
			return _StartWebsocket(string.Format("chat/rooms/{0}/", roomId.ToString()));
		}
		
		void IRoomWebSocketPresenter.SendMessage(string content)
		{
			_Send(new Dictionary<string, object>()
				{
					{"command", "send_message"},
					{"content", content},
				});
		}
		
		UniTask<OneOf<None, UniTaskError>> IRoomWebSocketPresenter.JoinRoom()
			=>
				_SendWaitTillReturn<None>(
					new Dictionary<string, object>()
					{
						{"command", "join_room"},
					});

		UniTask<OneOf<None, UniTaskError>> IRoomWebSocketPresenter.SendStartGame()
			=>
				_SendWaitTillReturn<None>(
					new Dictionary<string, object>()
					{
						{"command", "start_game"},
					});

		void IRoomWebSocketPresenter.RegisterOnReceiveAppendMessage(Action<MessageResult> onReceiveMessage)
		{
			_RegisterOnReceiveMessage("append_message", onReceiveMessage);
		}
		
		void IRoomWebSocketPresenter.RegisterOnReceiveUpdateRoom(Action<RoomResult> onReceiveMessage)
		{
			_RegisterOnReceiveMessage("update_room", onReceiveMessage);
		}

		void IRoomWebSocketPresenter.RegisterOnReceiveStartTTTGame(Action<TicTacToeGameResult> onReceiveMessage)
		{
			_RegisterOnReceiveMessage("start_ttt_game", onReceiveMessage);
		}

		void IRoomWebSocketPresenter.RegisterOnReceiveStartGOAGame(Action<GOAGameResult> onReceiveMessage)
		{
			_RegisterOnReceiveMessage("start_goa_game", onReceiveMessage);
		}
	}
}