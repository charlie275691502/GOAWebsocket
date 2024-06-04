using System;
using System.Collections.Generic;
using BestHTTP;
using Common;
using Common.UniTaskExtension;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using OneOf;
using OneOf.Types;

namespace Web
{
	public interface IHTTPPresenter
	{
		UniTask<OneOf<None, UniTaskError>> Login(string username, string password);
		UniTask<OneOf<None, UniTaskError>> RegisterThenLogin(string username, string password, string email);
		UniTask<OneOf<None, UniTaskError>> RefreshSelfPlayerData();
		UniTask<OneOf<None, UniTaskError>> PutSelfPlayerData(string nickName);
		UniTask<OneOf<RoomResult, UniTaskError>> CreateRoom(string roomName, string gameType, int playerPlot);
		UniTask<OneOf<RoomListResult, UniTaskError>> GetRoomList();
		UniTask<OneOf<RoomWithMessagesResult, UniTaskError>> GetRoomWithMessages(int roomId);
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
		
		UniTask<OneOf<None, UniTaskError>> IHTTPPresenter.Login(string username, string password)
			=>  
				_Send<LoginResult>(
					"auth/jwt/create/",
					HTTPMethods.Post,
					new Dictionary<string, object>()
					{
						{"username", username},
						{"password", password},
					},
					false)
				.OnSuccess(result => _backendPlayerPresenter.Accept(result))
				.ToNone();
		
		UniTask<OneOf<None, UniTaskError>> IHTTPPresenter.RegisterThenLogin(string username, string password, string email)
			=> 
				_Send(
					"auth/users/",
					HTTPMethods.Post,
					new Dictionary<string, object>()
					{
						{"username", username},
						{"password", password},
						{"email", email},
					},
					false)
				.OnSuccess(_ => (this as IHTTPPresenter).Login(username, password))
				.ToNone();

		UniTask<OneOf<None, UniTaskError>> IHTTPPresenter.RefreshSelfPlayerData()
			=> 
				_Send<PlayerDataResult>(
					"mainpage/players/me/",
					HTTPMethods.Get,
					new Dictionary<string, object>())
				.OnSuccess(result => _backendPlayerPresenter.Accept(result))
				.ToNone();

		UniTask<OneOf<None, UniTaskError>> IHTTPPresenter.PutSelfPlayerData(string nickName)
			=> 
				_Send(
					"mainpage/players/me/",
					HTTPMethods.Put,
					new Dictionary<string, object>()
					{
						{"nick_name", nickName},
					})
				.OnSuccess(_ => _backendPlayerPresenter.AcceptNickName(nickName));
		
		UniTask<OneOf<RoomResult, UniTaskError>> IHTTPPresenter.CreateRoom(string roomName, string gameType, int playerPlot)
			=> 
				_Send<RoomResult>(
					"chat/rooms/",
					HTTPMethods.Post,
					new Dictionary<string, object>()
					{
						{"room_name", roomName},
						{"game_setting", new Dictionary<string, object>
							{
								{"game_type", gameType},
								{"player_plot", playerPlot},
							}},
					});
		
		UniTask<OneOf<RoomListResult, UniTaskError>> IHTTPPresenter.GetRoomList()
			=> 
				_Send<RoomListResult>(
					"chat/rooms/",
					HTTPMethods.Get,
					new Dictionary<string, object>());
		
		UniTask<OneOf<RoomWithMessagesResult, UniTaskError>> IHTTPPresenter.GetRoomWithMessages(int roomId)
			=> 
				_Send<RoomWithMessagesResult>(
					string.Format("chat/rooms/{0}", roomId),
					HTTPMethods.Get,
					new Dictionary<string, object>());

		private UniTask<OneOf<None, UniTaskError>> _Send(string path, HTTPMethods method, Dictionary<string, object> body, bool needAuthorization = true)
		{
			return _SendRequest(path, method, body, needAuthorization).ToNone();
		}

		private async UniTask<OneOf<T, UniTaskError>> _Send<T>(string path, HTTPMethods method, Dictionary<string, object> body, bool needAuthorization = true)
		{
			var requestResultOpt = await _SendRequest(path, method, body, needAuthorization);
			return requestResultOpt.MapT0(rawString => JsonConvert.DeserializeObject<T>(rawString));
		}

		private async UniTask<OneOf<string, UniTaskError>> _SendRequest(string path, HTTPMethods method, Dictionary<string, object> body, bool needAuthorization)
		{
			var json = JsonConvert.SerializeObject(body);
			var isFinished = false;
			var result = string.Empty;
			var error = string.Empty;
			
			OnRequestFinishedDelegate onRequestFinished = (HTTPRequest request, HTTPResponse response) =>
			{
				if(response.IsSuccess)
				{
					result = response.DataAsText;
				} else 
				{
					error = string.Format("[{0}] {1}\n\n{2}", response.StatusCode, response.Message, response.DataAsText);
				}
				
				isFinished = true;
			};
			_loadingView.Enter();
			
			HTTPRequest request = new HTTPRequest(
				new Uri(string.Format("http://{0}/{1}", WebUtility.Domain, path)),
				method,
				onRequestFinished);
			request.RawData = System.Text.Encoding.UTF8.GetBytes(json);
			request.SetHeader("Content-Type", "application/json; charset=UTF-8");
			if(needAuthorization)
			{
				request.AddHeader("Authorization", string.Format("JWT {0}", _backendPlayerData.AccessKey));
			}
			
			_ = request.Send();
			
			while (!isFinished)
			{
				if(
					request.State == HTTPRequestStates.Error ||
					request.State == HTTPRequestStates.Aborted ||
					request.State == HTTPRequestStates.ConnectionTimedOut ||
					request.State == HTTPRequestStates.TimedOut)
				{
					error = "Request Finished with Error! " + (request.Exception != null ? (request.Exception.Message + "\n" + request.Exception.StackTrace) : "No Further Information");
					isFinished = true;
				}

				await UniTask.Yield();
			}
			
			_loadingView.Leave();
			
			if(!string.IsNullOrEmpty(error))
			{
				if(WebUtility.RequestDebugMode)
				{
					Debug.LogError(error);
				}
				return new UniTaskError(error);
			} else 
			{
				if(WebUtility.RequestDebugMode)
				{
					Debug.Log(result);
				}
				return result;
			}
		}
	}
}
