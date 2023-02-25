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
			_registerView.Enter(_OnSwitchToLogin, _OnRegister);
			_commandExecutor.Clear();
			yield return _commandExecutor.Start();
			ret.Accept(_result);
		}
		
		private void _Stop()
		{
			_registerView.Leave();
			_commandExecutor.Stop();
		}
		
		private void _OnSwitchToLogin()
		{
			_result = AuthorizationTabResult.ToLogin;
			_Stop();
		}
		
		private void _OnRegister()
		{
			Debug.Log("Register");
			_result = AuthorizationTabResult.Leave;
			_Stop();
		}
	}
}