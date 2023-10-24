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
		IEnumerator Run(IReturn<AuthorizationReturn> ret);
	}
	public class LoginPresenter : ILoginPresneter
	{
		private IHTTPPresenter _hTTPPresenter;
		private IWarningPresenter _warningPresenter;
		private ILoginView _view;
		
		private CommandExecutor _commandExecutor = new CommandExecutor();
		private AuthorizationReturn _result;
		
		public LoginPresenter(IHTTPPresenter hTTPPresenter, IWarningPresenter warningPresenter, ILoginView view)
		{
			_hTTPPresenter = hTTPPresenter;
			_warningPresenter = warningPresenter;
			_view = view;
		}
		
		public IEnumerator Run(IReturn<AuthorizationReturn> ret)
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
			_result = new AuthorizationReturn(AuthorizationReturnType.Register);
			_Stop();
		}
		
		private void _OnLogin(string username, string password)
		{
			_commandExecutor.TryAdd(_Login(username, password));
		}

		private IEnumerator _Login(string username, string password)
		{
			yield return _hTTPPresenter.Login(username, password).RunAndHandleInternetError(_warningPresenter);
			_result = new AuthorizationReturn(AuthorizationReturnType.EnterMetagame);
			_Stop();
		}
	}
}