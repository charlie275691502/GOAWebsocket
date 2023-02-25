using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Authorization
{
	public interface ILoginView
	{
		void Enter(Action onClickSwitchToRegister, Action onClickLogin);
		void Leave();
	}
	
	public class LoginView : MonoBehaviour, ILoginView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Button _switchToRegisterButton;
		[SerializeField]
		private Button _loginButton;
		
		private Action _onClickSwitchToRegister;
		private Action _onClickLogin;
		
		public void Enter(Action onClickSwitchToRegister, Action onClickLogin)
		{
			_Register(onClickSwitchToRegister, onClickLogin);
			_panel.SetActive(true);
		}
		
		public void Leave()
		{
			_Unregister();
			_panel.SetActive(false);
		}
		
		private void _Register(Action onClickSwitchToRegister, Action onClickLogin)
		{
			_onClickSwitchToRegister = onClickSwitchToRegister;
			_onClickLogin = onClickLogin;
			
			_switchToRegisterButton.onClick.AddListener(_OnClickSwitchToRegister);
			_loginButton.onClick.AddListener(_OnClickLogin);
		}
		
		private void _Unregister()
		{
			_onClickSwitchToRegister = null;
			_onClickLogin = null;
			
			_switchToRegisterButton.onClick.RemoveAllListeners();
			_loginButton.onClick.RemoveAllListeners();
		}
		
		private void _OnClickSwitchToRegister()
		{
			_onClickSwitchToRegister?.Invoke();
		}
		
		private void _OnClickLogin()
		{
			_onClickLogin?.Invoke();
		}
	}
}