using System.Collections;
using System.Collections.Generic;
using Common;
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
		private readonly IHTTPPresenter _hTTPPresenter;
		private readonly IWarningPresenter _warningPresenter;
		private readonly ILoginView _loginView;
		
		private CommandExecutor _commandExecutor = new CommandExecutor();
		private AuthorizationTabResult _result;
		
		public LoginPresenter(IHTTPPresenter hTTPPresenter, IWarningPresenter warningPresenter, ILoginView loginView)
		{
			_hTTPPresenter = hTTPPresenter;
			_warningPresenter = warningPresenter;
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
			var monad = _hTTPPresenter.Login();
			yield return monad.Do();
			if(monad.Error != null)
			{
				yield return _warningPresenter.Run("Error occurs when send to server", monad.Error.ToString());
				yield break;
			}
			Debug.Log("Request Finished! Text received: " + monad.Result);
			_result = AuthorizationTabResult.Leave;
			_Stop();
		}		
	}
}