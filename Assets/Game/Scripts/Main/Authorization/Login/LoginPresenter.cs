using System;
using Common;
using Common.UniTaskExtension;
using Common.Warning;
using Cysharp.Threading.Tasks;
using Web;

namespace Authorization.Login
{
	public interface ILoginPresneter: IAuthorizationSubTabPresenter
	{

	}

	public abstract record LoginState
	{
		public record Open() : LoginState;
		public record Idle() : LoginState;
		public record Login(string Username, string Password) : LoginState;
		public record SwitchToRegister() : LoginState;
		public record Close() : LoginState;
	}

	public record LoginProperty(LoginState State, string DefaultUsername, string DefaultPassword);

	public class LoginPresenter : ILoginPresneter
	{
		private IHTTPPresenter _hTTPPresenter;
		private ISetting _setting;
		private ILocalStorage _localStorage;
		private IWarningPresenter _warningPresenter;
		private ILoginView _view;

		private LoginProperty _prop;

		public LoginPresenter(IHTTPPresenter hTTPPresenter, ISetting setting, ILocalStorage localStorage, IWarningPresenter warningPresenter, ILoginView view)
		{
			_hTTPPresenter = hTTPPresenter;
			_setting = setting;
			_localStorage = localStorage;
			_warningPresenter = warningPresenter;
			_view = view;

			_view.RegisterCallback(
				(username, password) =>
					_ChangeStateIfIdle(new LoginState.Login(username, password)),
				() =>
					_ChangeStateIfIdle(new LoginState.SwitchToRegister()));
		}
		
		async UniTask<AuthorizationSubTabReturn> IAuthorizationSubTabPresenter.Run()
		{
			_prop = new LoginProperty(new LoginState.Open(), _localStorage.Username, _localStorage.Password);
			var ret = new AuthorizationSubTabReturn(new AuthorizationSubTabReturnType.Close());

			while (_prop.State is not LoginState.Close)
			{
				_view.Render(_prop);
				switch (_prop.State)
				{
					case LoginState.Open:
						_prop = _prop with { State = new LoginState.Idle() };
						break;

					case LoginState.Idle:
						break;

					case LoginState.Login info:
						var success = await _Login(info.Username, info.Password);
						if (success)
						{
							await _hTTPPresenter.Login(info.Username, info.Password).RunAndHandleInternetError(_warningPresenter);
							_localStorage.Username = info.Username;
							if (_setting.SavePassword)
							{
								_localStorage.Password = info.Password;
							}
							_prop = _prop with { State = new LoginState.Close() };
							ret = new AuthorizationSubTabReturn(new AuthorizationSubTabReturnType.Close());
						} else 
						{
							_prop = _prop with { State = new LoginState.Idle() };
						}
						
						break;

					case LoginState.SwitchToRegister:
						_prop = _prop with { State = new LoginState.Close() };
						ret = new AuthorizationSubTabReturn(new AuthorizationSubTabReturnType.Switch(new AuthorizationState.Register()));
						break;

					case LoginState.Close:
						break;

					default:
						break;
				}
				await UniTask.Yield();
			}

			_view.Render(_prop);
			return ret;
		}

		private void _ChangeStateIfIdle(LoginState targetState, Action onChangeStateSuccess = null)
		{
			if (_prop.State is not LoginState.Idle)
				return;

			onChangeStateSuccess?.Invoke();
			_prop = _prop with { State = targetState };
		}

		private async UniTask<bool> _Login(string username, string password)
			=> await _hTTPPresenter.Login(username, password)
				.RunAndHandleInternetError(_warningPresenter)
				.IsSuccess();
	}
}