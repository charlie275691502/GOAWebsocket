using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Authorization
{
	public interface IRegisterView
	{
		void Enter(Action onClickSwitchToLogin, Action onClickRegister);
		void Leave();
	}
	
	public class RegisterView : MonoBehaviour, IRegisterView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Button _switchToLoginButton;
		[SerializeField]
		private Button _registerButton;
		
		private Action _onClickSwitchToLogin;
		private Action _onClickRegister;
		
		public void Enter(Action onClickSwitchToLogin, Action onClickRegister)
		{
			_Register(onClickSwitchToLogin, onClickRegister);
			_panel.SetActive(true);
		}
		
		public void Leave()
		{
			_Unregister();
			_panel.SetActive(false);
		}
		
		private void _Register(Action onClickSwitchToLogin, Action onClickRegister)
		{
			_onClickSwitchToLogin = onClickSwitchToLogin;
			_onClickRegister = onClickRegister;
			
			_switchToLoginButton.onClick.AddListener(_OnClickSwitchToLogin);
			_registerButton.onClick.AddListener(_OnClickRegister);
		}
		
		private void _Unregister()
		{
			_onClickSwitchToLogin = null;
			_onClickRegister = null;
			
			_switchToLoginButton.onClick.RemoveAllListeners();
			_registerButton.onClick.RemoveAllListeners();
		}
		
		private void _OnClickSwitchToLogin()
		{
			_onClickSwitchToLogin?.Invoke();
		}
		
		private void _OnClickRegister()
		{
			_onClickRegister?.Invoke();
		}
	}
}