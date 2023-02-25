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
			_loginView.Enter(_OnSwitchToRegister, _OnLogin);
			_commandExecutor.Clear();
			yield return _commandExecutor.Start();
			ret.Accept(_result);
		}
		
		private void _Stop()
		{
			_loginView.Leave();
			_commandExecutor.Stop();
		}
		
		private void _OnSwitchToRegister()
		{
			_result = AuthorizationTabResult.ToRegister;
			_Stop();
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
			_Stop();
		}		
	}
}