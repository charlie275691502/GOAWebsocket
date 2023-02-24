using System.Collections;
using System.Collections.Generic;
using Rayark.Mast;
using UnityEngine;

namespace Authorization
{
	public interface IRegisterPresenter
	{
		IEnumerator Run(IReturn<AuthorizationTabResult> ret);
	}
	
	public class RegisterPresenter : IRegisterPresenter
	{
		private IRegisterView _registerView;
		private CommandExecutor _commandExecutor = new CommandExecutor();
		private AuthorizationTabResult _result;
		
		public RegisterPresenter(IRegisterView registerView)
		{
			_registerView = registerView;
		}
		
		public IEnumerator Run(IReturn<AuthorizationTabResult> ret)
		{
			_Register();
			_registerView.Enter();
			_commandExecutor.Clear();
			yield return _commandExecutor.Start();
			_Unregister();
			ret.Accept(_result);
		}
		
		private void _Register()
		{
			_registerView.OnClickSwitchToLogin += _OnSwitchToLogin;
			_registerView.OnClickRegister += _OnRegister;
		}
		
		private void _Unregister()
		{
			_registerView.OnClickSwitchToLogin -= _OnSwitchToLogin;
			_registerView.OnClickRegister -= _OnRegister;
		}
		
		private void _OnSwitchToLogin()
		{
			_result = AuthorizationTabResult.ToLogin;
			_Leave();
		}
		
		private void _OnRegister()
		{
			Debug.Log("Register");
			_result = AuthorizationTabResult.Leave;
			_Leave();
		}
		
		private void _Leave()
		{
			_registerView.Leave();
			_commandExecutor.Stop();
		}
	}
}