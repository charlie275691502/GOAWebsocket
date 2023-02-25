using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rayark.Mast;

namespace Authorization
{
	public enum AuthorizationTabResult
	{
		ToLogin,
		ToRegister,
		Leave,
	}
	
	public interface IAuthorizationPresenter
	{
		IEnumerator Run();
	}
	
	public class AuthorizationPresenter : IAuthorizationPresenter
	{
		private enum Tab
		{
			Login,
			Register,
		}
		
		private readonly IAuthorizationView _authorizationView;
		private readonly ILoginPresneter _loginPresneter;
		private readonly ILoginView _loginView;
		private readonly IRegisterPresenter _registerPresenter;
		private readonly IRegisterView _registerView;
		
		public AuthorizationPresenter(
			ILoginPresneter loginPresneter,
			ILoginView loginView,
			IRegisterPresenter registerPresenter,
			IRegisterView registerView,
			IAuthorizationView authorizationView)
		{
			_authorizationView = authorizationView;
			_loginPresneter = loginPresneter;
			_loginView = loginView;
			_registerPresenter = registerPresenter;
			_registerView = registerView;
		}
		
		public IEnumerator Run()
		{
			_authorizationView.Display();
			yield return _Run();
		}
		
		private IEnumerator _Run()
		{
			var nowTab = Tab.Login;
			while (true)
			{
				var monad = 
					(nowTab == Tab.Login) 
						? new BlockMonad<AuthorizationTabResult>(r => _loginPresneter.Run(r))
						: new BlockMonad<AuthorizationTabResult>(r => _registerPresenter.Run(r));
				yield return monad.Do();
				if (monad.Error != null)
				{
					Debug.LogError(monad.Error);
					break;
				}
				
				if (monad.Result == AuthorizationTabResult.Leave)
				{
					break;
				}
				
				nowTab = 
					(monad.Result == AuthorizationTabResult.ToLogin) 
						? Tab.Login
						: Tab.Register;
			}
		}
	}
}
