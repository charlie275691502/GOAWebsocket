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
		IEnumerator Run(IReturn<AuthorizationStatus> ret);
	}
	
	public class RegisterPresenter : IRegisterPresenter
	{
		private IHTTPPresenter _hTTPPresenter;
		private IWarningPresenter _warningPresenter;
		private IRegisterView _view;
		
		private CommandExecutor _commandExecutor = new CommandExecutor();
		private AuthorizationStatus _result;
		
		public RegisterPresenter(IHTTPPresenter hTTPPresenter, IWarningPresenter warningPresenter, IRegisterView view)
		{
			_hTTPPresenter = hTTPPresenter;
			_warningPresenter = warningPresenter;
			_view = view;
		}
		
		public IEnumerator Run(IReturn<AuthorizationStatus> ret)
		{
			_view.Enter(_OnSwitchToLogin, _OnRegister);
			_commandExecutor.Clear();
			yield return _commandExecutor.Start();
			ret.Accept(_result);
		}
		
		private void _Stop()
		{
			_view.Leave();
			_commandExecutor.Stop();
		}
		
		private void _OnSwitchToLogin()
		{
			_result = new AuthorizationStatus(AuthorizationStatusType.Login);
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
			
			yield return WebUtility.RunAndHandleInternetError(_hTTPPresenter.RegisterThenLogin(username, password, email), _warningPresenter);
			_result = new AuthorizationStatus(AuthorizationStatusType.EnterMetagame);
			_Stop();
		}
	}
}