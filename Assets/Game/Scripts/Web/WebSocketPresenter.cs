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
		IMonad<None> Run();
		void Stop();
	}
	
	public class WebSocketPresenter : IWebSocketPresenter
	{
		private ILoadingView _loadingView;
		private IBackendPlayerPresenter _backendPlayerPresenter;
		private BackendPlayerData _backendPlayerData;
		
		private CommandExecutor _commandExecutor = new CommandExecutor();
		private Exception _error;
		private WebSocket _webSocket;
		
		protected string _path = string.Empty;
		
		public WebSocketPresenter(ILoadingView loadingView, IBackendPlayerPresenter backendPlayerPresenter, BackendPlayerData backendPlayerData)
		{
			_loadingView = loadingView;
			_backendPlayerPresenter = backendPlayerPresenter;
			_backendPlayerData = backendPlayerData;
		}
		
		public IMonad<None> Run()
		{
			return new BlockMonad<None>(_Run);
		}
		
		public void Stop()
		{
			_webSocket = null;
			_commandExecutor.Stop();
		}
		
		private IEnumerator _Run(IReturn<None> ret)
		{
			if(_webSocket != null)
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
				
			_webSocket.OnOpen += _OnWebSocketOpen;
			_webSocket.OnMessage += _OnMessageReceived;
			_webSocket.OnBinary += _OnBinaryMessageReceived;
			_webSocket.OnClosed += _OnWebSocketClosed;
			_webSocket.OnError += _OnError;
				
			_error = null;
			_webSocket.Open();
			_commandExecutor.Clear();
			yield return _commandExecutor.Start();
			_webSocket.Close();
			
			if(_error != null)
			{
				ret.Fail(_error);
			}
			_webSocket = null;
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
			Stop();
		}
		
		protected void _Send(Dictionary<string, object> body)
		{
			if(_webSocket == null)
			{
				Debug.LogErrorFormat("WebSocket isn't running on path : {0}", _path);
				return;
			}
			
			var json = JsonConvert.SerializeObject(body);
			_webSocket.Send(json);
		}
	}
}
