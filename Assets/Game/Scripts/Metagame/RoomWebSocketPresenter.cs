using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Rayark.Mast;
using UnityEngine;
using Web;

namespace Metagame
{
	public interface IRoomWebSocketPresenter : IWebSocketPresenter
	{
		IMonad<None> Run(int roomId);
		void SendMessage(string message);
		IMonad<None> JoinRoomMonad(int roomId);
		void LeaveRoom(int roomId);
		void RegisterOnReceiveAppendMessage(Action<MessageResult> onReceiveMessage);
		void RegisterOnReceiveUpdateRoom(Action<RoomResult> onReceiveMessage);
	}
	
	public class RoomWebSocketPresenter : WebSocketPresenter, IRoomWebSocketPresenter
	{
		public RoomWebSocketPresenter(ILoadingView loadingView, IBackendPlayerPresenter backendPlayerPresenter, BackendPlayerData backendPlayerData) : base(loadingView, backendPlayerPresenter, backendPlayerData)
		{
		}
		
		public IMonad<None> Run(int roomId)
		{
			return _Run(string.Format("chat/rooms/{0}/", roomId.ToString()));
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
		
		public IMonad<None> JoinRoomMonad(int roomId)
		{
			var body = new Dictionary<string, object>()
			{
				{"command", "join_room"},
				{"room_id", roomId},
			};
			
			return _SendWaitTillReturnMonad<None>("join_room", body);
		}
		
		public void LeaveRoom(int roomId)
		{
			var body = new Dictionary<string, object>()
			{
				{"command", "leave_room"},
				{"room_id", roomId},
			};
			
			_Send(body);
		}
		
		public void RegisterOnReceiveAppendMessage(Action<MessageResult> onReceiveMessage)
		{
			_RegisterOnReceiveMessage<MessageResult>("append_message", onReceiveMessage);
		}
		
		public void RegisterOnReceiveUpdateRoom(Action<RoomResult> onReceiveMessage)
		{
			_RegisterOnReceiveMessage<RoomResult>("update_room", onReceiveMessage);
		}
	}
}