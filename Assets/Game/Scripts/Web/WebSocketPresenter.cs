using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using BestHTTP.WebSocket;
using Common;
using Newtonsoft.Json;
using Rayark.Mast;
using UnityEngine;

namespace Web
{
	public interface IWebSocketPresenter
	{
		void Stop();
	}
	
	public class WebSocketPresenter : IWebSocketPresenter
	{
		private ILoadingView _loadingView;
		private IBackendPlayerPresenter _backendPlayerPresenter;
		private BackendPlayerData _backendPlayerData;
		
		private Exception _error;
		private WebSocket _webSocket;
		
		protected string _path = string.Empty;
		
		public WebSocketPresenter(ILoadingView loadingView, IBackendPlayerPresenter backendPlayerPresenter, BackendPlayerData backendPlayerData)
		{
			_loadingView = loadingView;
			_backendPlayerPresenter = backendPlayerPresenter;
			_backendPlayerData = backendPlayerData;
		}
		
		protected IMonad<None> _Run(string path)
		{
			return new BlockMonad<None>(r => _Run(path, r));
		}
		
		public void Stop()
		{
			_webSocket.Close();
		}
		
		private IEnumerator _Run(string path, IReturn<None> ret)
		{
			_path = path;
			if(_webSocket != null && _webSocket.IsOpen)
			{
				var errorMessage = string.Format("WebSocket already running on path : {0}", _path);
				Debug.LogError(errorMessage);
				ret.Fail(new Exception(errorMessage));
				yield break;
			}
			
			_webSocket = new WebSocket(
				new Uri(string.Format("ws://{0}:{1}/ws/{2}", WebUtility.Host, WebUtility.Port, _path)),
				string.Format("http://{0}:{1}/", WebUtility.Host, WebUtility.Port),
				string.Empty);
				
			_webSocket.OnInternalRequestCreated += (ws, req) => req.AddHeader("authorization", _backendPlayerData.AccessKey);
			_webSocket.OnOpen += _OnWebSocketOpen;
			_webSocket.OnMessage += _OnMessageReceived;
			_webSocket.OnBinary += _OnBinaryMessageReceived;
			_webSocket.OnClosed += _OnWebSocketClosed;
			_webSocket.OnError += _OnError;
				
			_error = null;
			_webSocket.Open();
			while (!_webSocket.IsOpen && _error == null)
			{
				if(_error != null)
				{
					ret.Fail(_error);
					Stop();
					yield break;
				}
				yield return null;
			}
		}

		private void _OnWebSocketOpen(WebSocket webSocket)
		{
			if(WebUtility.RequestDebugMode)
			{
				Debug.LogFormat("WebSocket open on path : {0}", _path);
			}
		}
		
		private void _OnMessageReceived(WebSocket webSocket, string message)
		{
			if(WebUtility.RequestDebugMode)
			{
				Debug.LogFormat("Text Message received from path : {0}\nmessage : {1}", _path, message);
			}
		}
		
		private void _OnBinaryMessageReceived(WebSocket webSocket, byte[] message)
		{
			if(WebUtility.RequestDebugMode)
			{
				Debug.LogFormat("Binary Message received from path : {0}\nLength: ", _path, message.Length);
			}
		}
		
		private void _OnWebSocketClosed(WebSocket webSocket, UInt16 code, string message)
		{
			if(WebUtility.RequestDebugMode)
			{
				Debug.LogFormat("WebSocket is now Closed from path : {0}\nstatus code : {1}\nmessage : {2}", _path, code, message);
			}
		}
		
		private void _OnError(WebSocket webSocket, string error)
		{
			if(WebUtility.RequestDebugMode)
			{
				Debug.LogErrorFormat("Receive Error from path : {0}\nError : {1}", _path, error);
			}
			_error = new Exception(error);
		}
		
		protected void _Send(Dictionary<string, object> body)
		{
			if(!_webSocket.IsOpen)
			{
				Debug.LogErrorFormat("WebSocket isn't running on path : {0}", _path);
				return;
			}
			
			var json = JsonConvert.SerializeObject(body);
			_webSocket.Send(json);
		}
	}
}
