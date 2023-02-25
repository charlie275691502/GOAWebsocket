using System.Collections;
using System.Collections.Generic;
using Common;
using Rayark.Mast;
using UnityEngine;
using Web;

namespace Authorization
{
	public interface IRegisterPresenter
	{
		IEnumerator Run(IReturn<AuthorizationTabResult> ret);
	}
	
	public class RegisterPresenter : IRegisterPresenter
	{
		private readonly IHTTPPresenter _hTTPPresenter;
		private readonly IWarningPresenter _warningPresenter;
		private readonly IRegisterView _registerView;
		
		private CommandExecutor _commandExecutor = new CommandExecutor();
		private AuthorizationTabResult _result;
		
		public RegisterPresenter(IHTTPPresenter hTTPPresenter, IWarningPresenter warningPresenter, IRegisterView registerView)
		{
			_hTTPPresenter = hTTPPresenter;
			_warningPresenter = warningPresenter;
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