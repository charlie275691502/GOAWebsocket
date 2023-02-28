using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Web
{
	public interface IRoomWebSocketPresenter : IWebSocketPresenter
	{
		void Message(string message);
	}
	
	public class RoomWebSocketPresenter : WebSocketPresenter, IRoomWebSocketPresenter
	{
		public RoomWebSocketPresenter(ILoadingView loadingView, IBackendPlayerPresenter backendPlayerPresenter, BackendPlayerData backendPlayerData) : base(loadingView, backendPlayerPresenter, backendPlayerData)
		{
			_path = "rooms/lobby/";
		}
		
		public void Message(string message)
		{
			var body = new Dictionary<string, object>()
			{
				{"message", message},
			};
			
			_Send(body);
		}
	}
}