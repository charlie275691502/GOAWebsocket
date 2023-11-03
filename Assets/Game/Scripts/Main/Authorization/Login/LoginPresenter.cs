using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Common.Warning;
using Cysharp.Threading.Tasks;
using UnityEngine;
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

	public record LoginProperty(LoginState State);

	public class LoginPresenter : ILoginPresneter
	{
		private IHTTPPresenter _hTTPPresenter;
		private IWarningPresenter _warningPresenter;
		private ILoginView _view;

		private LoginProperty _prop;

		public LoginPresenter(IHTTPPresenter hTTPPresenter, IWarningPresenter warningPresenter, ILoginView view)
		{
			_hTTPPresenter = hTTPPresenter;
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
			_prop = new LoginProperty(new LoginState.Open());
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
						await _hTTPPresenter.Login(info.Username, info.Password).RunAndHandleInternetError(_warningPresenter);
						_prop = _prop with { State = new LoginState.Close() };
						ret = new AuthorizationSubTabReturn(new AuthorizationSubTabReturnType.Close());
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

			return ret;
		}

		private void _ChangeStateIfIdle(LoginState targetState, Action onChangeStateSuccess = null)
		{
			if (_prop.State is not LoginState.Idle)
				return;

			onChangeStateSuccess?.Invoke();
			_prop = _prop with { State = targetState };
		}
	}
}