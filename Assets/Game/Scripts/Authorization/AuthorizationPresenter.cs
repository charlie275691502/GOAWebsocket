using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rayark.Mast;

namespace Authorization
{
	public enum AuthorizationStatusType
	{
		Login,
		Register,
		EnterMetagame,
	}
	
	public class AuthorizationStatus
	{
		public AuthorizationStatusType Type;
		
		public AuthorizationStatus(AuthorizationStatusType type)
		{
			Type = type;
		}
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
			var nextStatus = new AuthorizationStatus(AuthorizationStatusType.Login);
			while (nextStatus.Type != AuthorizationStatusType.EnterMetagame)
			{
				var monad = 
					(nextStatus.Type == AuthorizationStatusType.Login) 
						? new BlockMonad<AuthorizationStatus>(r => _loginPresneter.Run(r))
						: new BlockMonad<AuthorizationStatus>(r => _registerPresenter.Run(r));
				yield return monad.Do();
				if (monad.Error != null)
				{
					Debug.LogError(monad.Error);
					break;
				}
				
				nextStatus = monad.Result;
			}
		}
	}
}
