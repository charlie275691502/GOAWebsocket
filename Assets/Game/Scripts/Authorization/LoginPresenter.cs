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
		private ILoginView _view;
		
		private CommandExecutor _commandExecutor = new CommandExecutor();
		private AuthorizationTabResult _result;
		
		public LoginPresenter(IHTTPPresenter hTTPPresenter, IWarningPresenter warningPresenter, ILoginView view)
		{
			_hTTPPresenter = hTTPPresenter;
			_warningPresenter = warningPresenter;
			_view = view;
		}
		
		public IEnumerator Run(IReturn<AuthorizationTabResult> ret)
		{
			_view.Enter(_OnSwitchToRegister, _OnLogin);
			_commandExecutor.Clear();
			yield return _commandExecutor.Start();
			ret.Accept(_result);
		}
		
		private void _Stop()
		{
			_view.Leave();
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
			yield return WebUtility.RunAndHandleInternetError(_hTTPPresenter.Login(username, password), _warningPresenter);
			_result = AuthorizationTabResult.LoginSuccess;
			_Stop();
		}
	}
}