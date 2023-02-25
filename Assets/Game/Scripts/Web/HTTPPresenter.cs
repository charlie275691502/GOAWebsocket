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
		IMonad<LoginResult> Login(string username, string password);
		IMonad<RegisterResult> Register(string username, string password, string email);
	}
	public class HTTPPresenter : IHTTPPresenter
	{
		private ILoadingView _loadingView;
		
		public HTTPPresenter(ILoadingView loadingView)
		{
			_loadingView = loadingView;
		}
		
		public IMonad<LoginResult> Login(string username, string password)
		{
			return _Send<LoginResult>(
				"auth/jwt/create/",
				HTTPMethods.Post,
				new Dictionary<string, object>()
				{
					{"username", username},
					{"password", password},
				});
		}
		
		public IMonad<RegisterResult> Register(string username, string password, string email)
		{
			return _Send<RegisterResult>(
				"auth/users/",
				HTTPMethods.Post,
				new Dictionary<string, object>()
				{
					{"username", username},
					{"password", password},
					{"email", email},
				});
		}
		
		private IMonad<T> _Send<T>(string path, HTTPMethods method, Dictionary<string, object> body)
		{
			return new BlockMonad<string>(r => _SendRequest(path, method, body, r))
				.Map(r => JsonConvert.DeserializeObject<T>(r));
		}
		
		private IEnumerator _SendRequest(string path, HTTPMethods method, Dictionary<string, object> body, IReturn<string> ret)
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
					error = new Exception(string.Format("[{0}] {1}", response.StatusCode, response.Message));
				}
				
				isFinished = true;
			};
			_loadingView.Enter();
			
			HTTPRequest request = new HTTPRequest(new Uri(string.Format("http://{0}:{1}/{2}", WebUtility.Host, WebUtility.Port, path)), method, onRequestFinished);
			request.SetHeader("Content-Type", "application/json; charset=UTF-8");
			request.RawData = System.Text.Encoding.UTF8.GetBytes(json);
			// request.AddHeader("Authorization", "JWT " + accessKey);
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
				ret.Fail(error);
			} else 
			{
				ret.Accept(result);
			}
		}
	}
}
