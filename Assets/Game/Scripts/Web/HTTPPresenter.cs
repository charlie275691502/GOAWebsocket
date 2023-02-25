using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using Common;
using Rayark.Mast;
using UnityEngine;

namespace Web
{
	public interface IHTTPPresenter
	{
		IMonad<string> Login();
	}
	public class HTTPPresenter : IHTTPPresenter
	{
		private readonly ILoadingView _loadingView;
		
		string accessKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ0b2tlbl90eXBlIjoiYWNjZXNzIiwiZXhwIjoxNjc3MjA3NzgyLCJqdGkiOiI4ODZjZWNjOTI1ZTI0MWMwOWU0ZmVkOGVkMTM3NDVhNyIsInVzZXJfaWQiOjR9.29WT5dPlJbuL9wuyR8xC1zJawyRnkFp4OMRVW1Fk6XY";
		
		public HTTPPresenter(ILoadingView loadingView)
		{
			_loadingView = loadingView;
		}
		
		public IMonad<string> Login()
		{
			return _Send<string>("auth/jwt/create/", HTTPMethods.Post, "{ \"username\": \"player1\", \"password\": \"asd860221\" }");
		}
		
		
		private IMonad<T> _Send<T>(string path, HTTPMethods method, string json)
		{
			return new BlockMonad<string>(r => _SendRequest(path, method, json, r))
				.Map(r => (T)(object)r);
		}
		
		private IEnumerator _SendRequest(string path, HTTPMethods method, string json, IReturn<string> ret)
		{
			var isFinished = false;
			var result = string.Empty;
			OnRequestFinishedDelegate onRequestFinished = (HTTPRequest request, HTTPResponse response) =>
			{
				result = response.DataAsText;
				isFinished = true;
				_loadingView.Leave();
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
					ret.Fail(new Exception("Request Finished with Error! " + (request.Exception != null ? (request.Exception.Message + "\n" + request.Exception.StackTrace) : "No Exception")));
					_loadingView.Leave();
					yield break;
				}
				
				yield return null;
			}
			
			ret.Accept(result);
		}
	}
}
