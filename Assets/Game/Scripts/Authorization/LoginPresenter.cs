using System.Collections;
using System.Collections.Generic;
using Rayark.Mast;
using UnityEngine;
using Web;

namespace Authorization
{
	public interface ILoginPresneter
	{
		IEnumerator Run(IReturn<AuthorizationTabResult> ret);
	}
	public class LoginPresenter : ILoginPresneter
	{
		private IHTTPPresenter _hTTPPresenter;
		private ILoginView _loginView;
		private CommandExecutor _commandExecutor = new CommandExecutor();
		private AuthorizationTabResult _result;
		
		public LoginPresenter(IHTTPPresenter hTTPPresenter, ILoginView loginView)
		{
			_hTTPPresenter = hTTPPresenter;
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
			if(_commandExecutor.Empty)
				_commandExecutor.Add(_Login());
		}

		private IEnumerator _Login()
		{
			var monad = _hTTPPresenter.TestSend();
			yield return monad.Do();
			if(monad.Error != null)
			{
				Debug.LogError(monad.Error);
				yield break;
			}
			Debug.Log("Request Finished! Text received: " + monad.Result);
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