using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Common.UniTaskExtension;
using Cysharp.Threading.Tasks;
using OneOf;
using OneOf.Types;
using Optional;
using UnityEngine;
using Web;

namespace Metagame
{
	public interface IRoomWebSocketPresenter : IWebSocketPresenter
	{
		UniTask<OneOf<None, UniTaskError>> Start(int roomId);
		void SendMessage(string message);
		UniTask<OneOf<None, UniTaskError>> JoinRoom(int roomId);
		UniTask<OneOf<None, UniTaskError>> LeaveRoom(int roomId);
		void RegisterOnReceiveAppendMessage(Action<MessageResult> onReceiveMessage);
		void RegisterOnReceiveUpdateRoom(Action<RoomResult> onReceiveMessage);
	}
	
	public class RoomWebSocketPresenter : WebSocketPresenter, IRoomWebSocketPresenter
	{
		public RoomWebSocketPresenter(ILoadingView loadingView, IBackendPlayerPresenter backendPlayerPresenter, BackendPlayerData backendPlayerData) : base(loadingView, backendPlayerPresenter, backendPlayerData)
		{
		}

		public UniTask<OneOf<None, UniTaskError>> Start(int roomId)
		{
			return _StartWebsocket(string.Format("chat/rooms/{0}/", roomId.ToString()));
		}
		
		public void SendMessage(string content)
		{
			var body = new Dictionary<string, object>()
			{
				{"command", "send_message"},
				{"content", content},
			};
			
			_Send(body);
		}
		
		public UniTask<OneOf<None, UniTaskError>> JoinRoom(int roomId)
			=>
				_SendWaitTillReturn<None>(
					"join_room", 
					new Dictionary<string, object>()
					{
						{"command", "join_room"},
						{"room_id", roomId},
					});
		
		public UniTask<OneOf<None, UniTaskError>> LeaveRoom(int roomId)
			=>
				_SendWaitTillReturn<None>(
					"leave_room", 
					new Dictionary<string, object>()
					{
						{"command", "leave_room"},
						{"room_id", roomId},
					});

		public void RegisterOnReceiveAppendMessage(Action<MessageResult> onReceiveMessage)
		{
			_RegisterOnReceiveMessage("append_message", onReceiveMessage);
		}
		
		public void RegisterOnReceiveUpdateRoom(Action<RoomResult> onReceiveMessage)
		{
			_RegisterOnReceiveMessage("update_room", onReceiveMessage);
		}
	}
}