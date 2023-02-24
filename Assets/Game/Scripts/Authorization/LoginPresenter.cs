using System.Collections;
using System.Collections.Generic;
using Rayark.Mast;
using UnityEngine;

namespace Authorization
{
	public interface ILoginPresneter
	{
		IEnumerator Run(IReturn<AuthorizationTabResult> ret);
	}
	public class LoginPresenter : ILoginPresneter
	{
		private ILoginView _loginView;
		private CommandExecutor _commandExecutor = new CommandExecutor();
		private AuthorizationTabResult _result;
		
		public LoginPresenter(ILoginView loginView)
		{
			_loginView = loginView;
		}
		
		public IEnumerator Run(IReturn<AuthorizationTabResult> ret)
		{
			_Register();
			_loginView.Enter();
			_commandExecutor.Clear();
			yield return _commandExecutor.Start();
			_Unregister();
			ret.Accept(_result);
		}
		
		private void _Register()
		{
			_loginView.OnClickSwitchToRegister += _OnSwitchToRegister;
			_loginView.OnClickLogin += _OnLogin;
		}
		
		private void _Unregister()
		{
			_loginView.OnClickSwitchToRegister -= _OnSwitchToRegister;
			_loginView.OnClickLogin -= _OnLogin;
		}
		
		private void _OnSwitchToRegister()
		{
			_result = AuthorizationTabResult.ToRegister;
			_Leave();
		}
		
		private void _OnLogin()
		{
			Debug.Log("Login");
			_result = AuthorizationTabResult.Leave;
			_Leave();
		}
		
		private void _Leave()
		{
			_loginView.Leave();
			_commandExecutor.Stop();
		}
	}
}