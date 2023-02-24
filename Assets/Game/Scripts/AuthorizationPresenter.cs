using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rayark.Mast;

namespace Authorization
{
	public interface IAuthorizationPresenter
	{
		IEnumerator Run();
	}
	public class AuthorizationPresenter : IAuthorizationPresenter
	{
		private IAuthorizationView _authorizationView;
		private SimpleExecutor _executor = new SimpleExecutor();
				
		public AuthorizationPresenter(IAuthorizationView authorizationView)
		{
			_authorizationView = authorizationView;
		}
		
		public IEnumerator Run()
		{
			_authorizationView.Display();
			yield return _executor.Start();
		}
	}
}
