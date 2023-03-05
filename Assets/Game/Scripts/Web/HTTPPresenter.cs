using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using Common;
using Newtonsoft.Json;
using Rayark.Mast;
using UnityEngine;

namespace Web
{
	public interface IHTTPPresenter
	{
		IMonad<None> Login(string username, string password);
		IMonad<None> RegisterThenLogin(string username, string password, string email);
		IMonad<None> GetSelfPlayerData();
		IMonad<None> UpdateSelfPlayerData(string nickName);
		IMonad<RoomListResult> GetRoomList();
		IMonad<RoomWithMessagesResult> GetRoomWithMessages(int roomId);
	}
	public class HTTPPresenter : IHTTPPresenter
	{
		private ILoadingView _loadingView;
		private IBackendPlayerPresenter _backendPlayerPresenter;
		private BackendPlayerData _backendPlayerData;
		
		public HTTPPresenter(ILoadingView loadingView, IBackendPlayerPresenter backendPlayerPresenter, BackendPlayerData backendPlayerData)
		{
			_loadingView = loadingView;
			_backendPlayerPresenter = backendPlayerPresenter;
			_backendPlayerData = backendPlayerData;
		}
		
		public IMonad<None> Login(string username, string password)
		{
			return 
				_Send<LoginResult>(
					"auth/jwt/create/",
					HTTPMethods.Post,
					new Dictionary<string, object>()
					{
						{"username", username},
						{"password", password},
					},
					false)
				.Then(result => _backendPlayerPresenter.Accept(result));
		}
		
		public IMonad<None> RegisterThenLogin(string username, string password, string email)
		{
			return 
				_Send<None>(
					"auth/users/",
					HTTPMethods.Post,
					new Dictionary<string, object>()
					{
						{"username", username},
						{"password", password},
						{"email", email},
					},
					false)
				.Then(r => Login(username, password));
		}
		
		public IMonad<None> GetSelfPlayerData()
		{
			return 
				_Send<PlayerDataResult>(
					"mainpage/players/me/",
					HTTPMethods.Get,
					new Dictionary<string, object>())
				.Then(r => _backendPlayerPresenter.Accept(r));
		}
		
		public IMonad<None> UpdateSelfPlayerData(string nickName)
		{
			return 
				_Send<None>(
					"mainpage/players/me/",
					HTTPMethods.Put,
					new Dictionary<string, object>()
					{
						{"nick_name", nickName},
					})
				.Then(r => _backendPlayerPresenter.AcceptNickName(nickName));
		}
		
		public IMonad<RoomListResult> GetRoomList()
		{
			return 
				_Send<RoomListResult>(
					"chat/rooms/",
					HTTPMethods.Get,
					new Dictionary<string, object>());
		}
		
		public IMonad<RoomWithMessagesResult> GetRoomWithMessages(int roomId)
		{
			return 
				_Send<RoomWithMessagesResult>(
					string.Format("chat/rooms/{0}", roomId),
					HTTPMethods.Get,
					new Dictionary<string, object>());
		}
		
		private IMonad<T> _Send<T>(string path, HTTPMethods method, Dictionary<string, object> body, bool needAuthorization = true)
		{
			return new BlockMonad<string>(r => _SendRequest(path, method, body, needAuthorization, r))
				.Map(r => JsonConvert.DeserializeObject<T>(r));
		}
		
		private IEnumerator _SendRequest(string path, HTTPMethods method, Dictionary<string, object> body, bool needAuthorization, IReturn<string> ret)
		{
			var json = JsonConvert.SerializeObject(body);
			var isFinished = false;
			var result = string.Empty;
			Exception error = null;
			
			OnRequestFinishedDelegate onRequestFinished = (HTTPRequest request, HTTPResponse response) =>
			{
				if(response.IsSuccess)
				{
					result = response.DataAsText;
				} else 
				{
					error = new Exception(string.Format("[{0}] {1}\n\n{2}", response.StatusCode, response.Message, response.DataAsText));
				}
				
				isFinished = true;
			};
			_loadingView.Enter();
			
			HTTPRequest request = new HTTPRequest(
				new Uri(string.Format("http://{0}:{1}/{2}", WebUtility.Host, WebUtility.Port, path)),
				method,
				onRequestFinished);
			request.RawData = System.Text.Encoding.UTF8.GetBytes(json);
			request.SetHeader("Content-Type", "application/json; charset=UTF-8");
			if(needAuthorization)
			{
				request.AddHeader("Authorization", string.Format("JWT {0}", _backendPlayerData.AccessKey));
			}
			
			request.Send();
			
			while (!isFinished)
			{
				if(
					request.State == HTTPRequestStates.Error ||
					request.State == HTTPRequestStates.Aborted ||
					request.State == HTTPRequestStates.ConnectionTimedOut ||
					request.State == HTTPRequestStates.TimedOut)
				{
					error = new Exception("Request Finished with Error! " + (request.Exception != null ? (request.Exception.Message + "\n" + request.Exception.StackTrace) : "No Exception"));
					isFinished = true;
				}
				
				yield return null;
			}
			
			_loadingView.Leave();
			
			if(error != null)
			{
				if(WebUtility.RequestDebugMode)
				{
					Debug.LogError(error);
				}
				ret.Fail(error);
			} else 
			{
				if(WebUtility.RequestDebugMode)
				{
					Debug.Log(result);
				}
				ret.Accept(result);
			}
		}
	}
}
