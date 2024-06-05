using Cysharp.Threading.Tasks;
using Authorization.Login;
using Authorization.Register;
using Main;
using System;

namespace Authorization
{
	public record AuthorizationState
	{
		public record Open() : AuthorizationState;
		public record Idle() : AuthorizationState;
		public record Login() : AuthorizationState;
		public record Register() : AuthorizationState;
		public record Close() : AuthorizationState;
	}

	public record AuthorizationProperty(AuthorizationState State);

	public interface IAuthorizationPresenter
	{
		UniTask<MainSubTabReturn> Run();
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
		private IAuthorizationView _view;
		
		private AuthorizationProperty _prop;
		
		public AuthorizationPresenter(
			ILoginPresneter loginPresneter,
			IRegisterPresenter registerPresenter,
			IAuthorizationView view)
		{
			_loginPresneter = loginPresneter;
			_registerPresenter = registerPresenter;
			_view = view;
			
			_view.RegisterCallback(
				() => 
					_ChangeStateIfIdle(new AuthorizationState.Login()));
		}
		
		async UniTask<MainSubTabReturn> IAuthorizationPresenter.Run()
		{
			_prop = new AuthorizationProperty(new AuthorizationState.Open());
			
			while (_prop.State is not AuthorizationState.Close)
			{
				_view.Render(_prop);
				
				switch (_prop.State)
				{
					case AuthorizationState.Open:
						_prop = _prop with { State = new AuthorizationState.Idle() };
						break;
						
					case AuthorizationState.Idle:
						break;
						
					case AuthorizationState.Login:
						_UpdateState(await _loginPresneter.Run());
						break;
						
					case AuthorizationState.Register:
						_UpdateState(await _registerPresenter.Run());
						break;
						
					case AuthorizationState.Close:
						break;
				}
				await UniTask.Yield();
			}
			
			_view.Render(_prop);
			return new MainSubTabReturn(new MainSubTabReturnType.Switch(new MainState.Metagame()));
		}

		private void _ChangeStateIfIdle(AuthorizationState targetState, Action onChangeStateSuccess = null)
		{
			if (_prop.State is not AuthorizationState.Idle)
				return;

			onChangeStateSuccess?.Invoke();
			_prop = _prop with { State = targetState };
		}

		private void _UpdateState(AuthorizationSubTabReturn subTabReturn)
		{
			switch (subTabReturn.Type)
			{
				case AuthorizationSubTabReturnType.Close:
					_prop = _prop with { State = new AuthorizationState.Close() };
					break;
				case AuthorizationSubTabReturnType.Switch info:
					_prop = _prop with { State = info.State };
					break;
			};
		}
	}
}
