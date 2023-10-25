using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Authorization.Login;
using Authorization.Register;

namespace Authorization
{
	public enum AuthorizationReturnType
	{
		Login,
		Register,
		EnterMetagame,
	}
	
	public record AuthorizationReturn(AuthorizationReturnType Type);
	
	public interface IAuthorizationPresenter
	{
		UniTask Run();
	}

	public interface IAuthorizationSubTabPresenter
	{
		UniTask<AuthorizationReturnType> Run();
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
				var nextType = await _GetCurrentSubTabPresenter(nextStatus.Type).Run();
				nextStatus = nextStatus with { Type = nextType };
			}
		}

		private IAuthorizationSubTabPresenter _GetCurrentSubTabPresenter(AuthorizationReturnType type)
			=> type switch
			{
				AuthorizationReturnType.Login => _loginPresneter,
				AuthorizationReturnType.Register => _registerPresenter,
				_ => null
			};
	}
}
