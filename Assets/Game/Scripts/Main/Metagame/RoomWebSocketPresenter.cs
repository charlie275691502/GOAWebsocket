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
		UniTask<OneOf<None, UniTaskError>> LeaveRoom();
		void RegisterOnReceiveAppendMessage(Action<MessageResult> onReceiveMessage);
		void RegisterOnReceiveUpdateRoom(Action<RoomResult> onReceiveMessage);
	}
	
	public class RoomWebSocketPresenter : WebSocketPresenter, IRoomWebSocketPresenter
	{
		public RoomWebSocketPresenter(BackendPlayerData backendPlayerData) : base(backendPlayerData)
		{
		}

		UniTask<OneOf<None, UniTaskError>> IRoomWebSocketPresenter.Start(int roomId)
		{
			return _StartWebsocket(string.Format("chat/rooms/{0}/", roomId.ToString()));
		}
		
		void IRoomWebSocketPresenter.SendMessage(string content)
		{
			var body = new Dictionary<string, object>()
			{
				{"command", "send_message"},
				{"content", content},
			};
			
			_Send(body);
		}
		
		UniTask<OneOf<None, UniTaskError>> IRoomWebSocketPresenter.JoinRoom()
			=>
				_SendWaitTillReturn<None>(
					"join_room", 
					new Dictionary<string, object>()
					{
						{"command", "join_room"},
					});
		
		UniTask<OneOf<None, UniTaskError>> IRoomWebSocketPresenter.LeaveRoom()
			=>
				_SendWaitTillReturn<None>(
					"leave_room", 
					new Dictionary<string, object>()
					{
						{"command", "leave_room"},
					});

		void IRoomWebSocketPresenter.RegisterOnReceiveAppendMessage(Action<MessageResult> onReceiveMessage)
		{
			_RegisterOnReceiveMessage("append_message", onReceiveMessage);
		}
		
		void IRoomWebSocketPresenter.RegisterOnReceiveUpdateRoom(Action<RoomResult> onReceiveMessage)
		{
			_RegisterOnReceiveMessage("update_room", onReceiveMessage);
		}
	}
}