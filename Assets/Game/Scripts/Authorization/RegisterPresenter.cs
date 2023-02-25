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
		private IHTTPPresenter _hTTPPresenter;
		private IWarningPresenter _warningPresenter;
		private IRegisterView _registerView;
		
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
		
		private void _OnRegister(string username, string password, string confirmPassword, string email)
		{
			if(_commandExecutor.Empty)
				_commandExecutor.Add(_Register(username, password, confirmPassword, email));
		}

		private IEnumerator _Register(string username, string password, string confirmPassword, string email)
		{
			if(password != confirmPassword)
			{
				yield return _warningPresenter.Run("Input Fail", "Passwrod and Confirm Password are not the same");
				yield break;
			}
			
			var monad = _hTTPPresenter.Register(username, password, email);
			yield return monad.Do();
			if(monad.Error != null)
			{
				yield return _warningPresenter.Run("Error occurs when send to server", monad.Error.ToString());
				yield break;
			}
			Debug.LogFormat("username: {0}", monad.Result.Username);
			_result = AuthorizationTabResult.ToLogin;
			_Stop();
		}
	}
}