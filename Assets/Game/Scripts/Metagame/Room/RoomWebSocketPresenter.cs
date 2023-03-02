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
		void Message(string message);
		void RegisterOnReceiveMessage(Action<MessageResult> onReceiveMessage);
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
		
		public void Message(string content)
		{
			var body = new Dictionary<string, object>()
			{
				{"command", "send_message"},
				{"content", content},
			};
			
			_Send(body);
		}
		
		public void RegisterOnReceiveMessage(Action<MessageResult> onReceiveMessage)
		{
			_RegisterOnReceiveMessage<MessageResult>("send_message", onReceiveMessage);
		}
	}
}