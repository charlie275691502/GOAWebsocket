using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Authorization
{
	public enum AuthorizationReturnType
	{
		Login,
		Register,
		EnterMetagame,
	}
	
	public class AuthorizationReturn
	{
		public AuthorizationReturnType Type;
		
		public AuthorizationReturn(AuthorizationReturnType type)
		{
			Type = type;
		}
	}
	
	public interface IAuthorizationPresenter
	{
		UniTask Run();
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
		
		public async UniTask Run()
		{
			var nextStatus = new AuthorizationReturn(AuthorizationReturnType.Login);
			while (nextStatus.Type != AuthorizationReturnType.EnterMetagame)
			{
				var monad = 
					(nextStatus.Type == AuthorizationReturnType.Login) 
						? new BlockMonad<AuthorizationReturn>(r => _loginPresneter.Run(r))
						: new BlockMonad<AuthorizationReturn>(r => _registerPresenter.Run(r));
				await monad.Do();
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
