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
		IMonad<string> TestSend();
	}
	public class HTTPPresenter : IHTTPPresenter
	{
		private readonly ILoadingView _loadingView;
		
		string accessKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ0b2tlbl90eXBlIjoiYWNjZXNzIiwiZXhwIjoxNjc3MjA3NzgyLCJqdGkiOiI4ODZjZWNjOTI1ZTI0MWMwOWU0ZmVkOGVkMTM3NDVhNyIsInVzZXJfaWQiOjR9.29WT5dPlJbuL9wuyR8xC1zJawyRnkFp4OMRVW1Fk6XY";
		
		public HTTPPresenter(ILoadingView loadingView)
		{
			_loadingView = loadingView;
		}
		
		public IMonad<string> TestSend()
		{
			return _Send<string>();
		}
		
		
		private IMonad<T> _Send<T>()
		{
			return new BlockMonad<string>(r => _SendRequest(r))
				.Map(r => (T)(object)r);
		}
		
		private IEnumerator _SendRequest(IReturn<string> ret)
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
			
			HTTPRequest request = new HTTPRequest(new Uri(string.Format("http://{0}/mainpage/players/me/", WebUtility.Address)), onRequestFinished);
			request.AddHeader("Authorization", "JWT " + accessKey);
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
