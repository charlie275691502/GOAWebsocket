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
		private IHTTPPresenter _hTTPPresenter;
		private IWarningPresenter _warningPresenter;
		private ILoginView _loginView;
		
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
		
		private void _OnLogin(string username, string password)
		{
			if(_commandExecutor.Empty)
				_commandExecutor.Add(_Login(username, password));
		}

		private IEnumerator _Login(string username, string password)
		{
			var monad = _hTTPPresenter.Login(username, password);
			yield return monad.Do();
			if(monad.Error != null)
			{
				yield return _warningPresenter.Run("Error occurs when send to server", monad.Error.Message.ToString());
				yield break;
			}
			Debug.LogFormat("access: {0}\n\nrefresh: {1}", monad.Result.AccessKey, monad.Result.RefreshKey);
			_result = AuthorizationTabResult.Leave;
			_Stop();
		}
	}
}