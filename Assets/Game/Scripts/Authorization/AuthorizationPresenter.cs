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
		LoginSuccess,
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
		
		private ILoginPresneter _loginPresneter;
		private IRegisterPresenter _registerPresenter;
		
		public AuthorizationPresenter(
			ILoginPresneter loginPresneter,
			IRegisterPresenter registerPresenter)
		{
			_loginPresneter = loginPresneter;
			_registerPresenter = registerPresenter;
		}
		
		public IEnumerator Run()
		{
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
				
				if (monad.Result == AuthorizationTabResult.LoginSuccess)
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
