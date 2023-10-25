using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using BestHTTP.WebSocket;
using Common;
using Common.UniTaskExtension;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneOf;
using OneOf.Types;
using Optional;
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
		
		private string _error = string.Empty;
		private WebSocket _webSocket;
		private Dictionary<string, Action<JToken>> _onReceiveMessageActionDic = new Dictionary<string, Action<JToken>>();
		
		protected string _path = string.Empty;
		
		public WebSocketPresenter(ILoadingView loadingView, IBackendPlayerPresenter backendPlayerPresenter, BackendPlayerData backendPlayerData)
		{
			_loadingView = loadingView;
			_backendPlayerPresenter = backendPlayerPresenter;
			_backendPlayerData = backendPlayerData;
		}
		
		public void Stop()
		{
			_onReceiveMessageActionDic.Clear();
			_webSocket.Close();
		}

		protected async UniTask<OneOf<None, UniTaskError>> _StartWebsocket(string path)
		{
			_path = path;
			if(_webSocket != null && _webSocket.IsOpen)
			{
				var errorMessage = string.Format("WebSocket already running on path : {0}", _path);
				Debug.LogError(errorMessage);
				return new UniTaskError(errorMessage);
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
				
			_error = string.Empty;
			_webSocket.Open();
			while (!_webSocket.IsOpen)
			{
				if(!string.IsNullOrEmpty(_error))
				{
					return new UniTaskError(_error);
				}
				await UniTask.Yield();
			}

			return default(None);
		}

		protected async UniTask<OneOf<T, UniTaskError>> _SendWaitTillReturn<T>(string command, Dictionary<string, object> body)
		{
			string successCommand = command + "_success";
			string failCommand = command + "_fail";
			var leave = false;
			var ret = new OneOf<T, UniTaskError>();

			_RegisterOnReceiveMessage<T>(successCommand, (result) => 
			{
				leave = true;
				ret = result;
			});
			
			_RegisterOnReceiveMessage<string>(failCommand, (message) => 
			{
				leave = true;
				ret = new UniTaskError(message);
			});
			
			_Send(body);
			
			while (!leave)
			{
				await UniTask.Yield();
			}
			
			_UnregisterOnReceiveMessage(successCommand);
			_UnregisterOnReceiveMessage(failCommand);

			return ret;
		}
		
		protected void _RegisterOnReceiveMessage<T>(string command, Action<T> action)
		{
			_onReceiveMessageActionDic[command] = (jtoken) => action?.Invoke(jtoken.HasValues ? jtoken.ToObject<T>() : default(T));
		}
		
		protected void _UnregisterOnReceiveMessage(string command)
		{
			_onReceiveMessageActionDic[command] = null;
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
			
			var jObject = JObject.Parse(message);
			var command = jObject["command"].ToObject<string>();
			var data = jObject["data"];
			
			if (_onReceiveMessageActionDic.TryGetValue(command, out var action))
			{
				action?.Invoke(data);
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
			_error = error;
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
