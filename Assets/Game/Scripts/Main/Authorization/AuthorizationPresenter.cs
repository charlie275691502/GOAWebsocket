using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Authorization.Login;
using Authorization.Register;

namespace Authorization
{
	public record AuthorizationState
	{
		public record Login() : AuthorizationState;
		public record Register() : AuthorizationState;
		public record Close() : AuthorizationState;
	}

	public record AuthorizationProperty(AuthorizationState State);

	public interface IAuthorizationPresenter : IMainSubTabPresenter
	{

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
		
		async UniTask<MainSubTabReturn> IMainSubTabPresenter.Run()
		{
			var prop = new AuthorizationProperty(new AuthorizationState.Login());
			while (prop.State is not AuthorizationState.Close)
			{
				var subTabReturn = await _GetCurrentSubTabPresenter(prop.State).Run();

				switch (subTabReturn.Type)
                {
					case AuthorizationSubTabReturnType.Close:
						prop = prop with { State = new AuthorizationState.Close() };
						break;
					case AuthorizationSubTabReturnType.Switch info:
						prop = prop with { State = info.State };
						break;
				}
			}
			return new MainSubTabReturn(new MainSubTabReturnType.Switch(new MainState.Metagame()));
		}

		private IAuthorizationSubTabPresenter _GetCurrentSubTabPresenter(AuthorizationState type)
			=> type switch
			{
				AuthorizationState.Login => _loginPresneter,
				AuthorizationState.Register => _registerPresenter,
				_ => null
			};
	}
}
