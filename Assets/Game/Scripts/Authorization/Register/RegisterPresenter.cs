using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Common.Warning;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Web;

namespace Authorization.Register
{
	public interface IRegisterPresenter: IAuthorizationSubTabPresenter
	{

	}
	
	public abstract record RegisterState
	{
		public record Open() : RegisterState;
		public record Idle() : RegisterState;
		public record Register(string Username, string Password, string ConfirmPassword, string Email) : RegisterState;
		public record SwitchToLogin() : RegisterState;
		public record Close() : RegisterState;
	}

	public record RegisterProperty(RegisterState State);
	
	public class RegisterPresenter : IRegisterPresenter
	{
		private IHTTPPresenter _hTTPPresenter;
		private IWarningPresenter _warningPresenter;
		private IRegisterView _view;
		
		private RegisterProperty _prop;
		
		public RegisterPresenter(IHTTPPresenter hTTPPresenter, IWarningPresenter warningPresenter, IRegisterView view)
		{
			_hTTPPresenter = hTTPPresenter;
			_warningPresenter = warningPresenter;
			_view = view;
			
			_view.RegisterCallback(
				(username, password, confirmPassword, email) => 
					_ChangeStateIfIdle(new RegisterState.Register(username, password, confirmPassword, email)),
				() => 
					_ChangeStateIfIdle(new RegisterState.SwitchToLogin()));
		}
		
		async UniTask<AuthorizationSubTabReturn> IAuthorizationSubTabPresenter.Run()
		{
			_prop = new RegisterProperty(new RegisterState.Open());
			var ret = new AuthorizationSubTabReturn(new AuthorizationSubTabReturnType.Close());

			while (_prop.State is not RegisterState.Close)
			{
				_view.Render(_prop);
				switch (_prop.State)
				{
					case RegisterState.Open:
						_prop = _prop with { State = new RegisterState.Idle() };
						break;
						
					case RegisterState.Idle:
						break;

					case RegisterState.Register info:
						await _Register(info.Username, info.Password, info.ConfirmPassword, info.Email);
						_prop = _prop with { State = new RegisterState.Close() };
						ret = new AuthorizationSubTabReturn(new AuthorizationSubTabReturnType.Close());
						break;
					
					case RegisterState.SwitchToLogin:
						_prop = _prop with { State = new RegisterState.Close() };
						ret = new AuthorizationSubTabReturn(new AuthorizationSubTabReturnType.Switch(new AuthorizationState.Login()));
						break;

					case RegisterState.Close:
						_prop = _prop with { 
							State = new RegisterState.Close() };
						break;
						
					default:
						break;
				}
				await UniTask.Yield();
			}
			
			return ret;
		}

		private void _ChangeStateIfIdle(RegisterState targetState, Action onChangeStateSuccess = null)
		{
			if (_prop.State is not RegisterState.Idle)
				return;

			onChangeStateSuccess?.Invoke();
			_prop = _prop with { State = targetState };
		}

		private async UniTask _Register(string username, string password, string confirmPassword, string email)
		{
			if(password != confirmPassword)
			{
				await _warningPresenter.Run("Input Fail", "Passwrod and Confirm Password are not the same");
				return;
			}
			
			await _hTTPPresenter.RegisterThenLogin(username, password, email).RunAndHandleInternetError(_warningPresenter);
		}
	}
}