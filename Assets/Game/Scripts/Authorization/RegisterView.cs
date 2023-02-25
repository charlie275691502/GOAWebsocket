using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Authorization
{
	public interface IRegisterView
	{
		void Enter(Action onSwitchToLogin, Action onRegister);
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
		
		private Action _onSwitchToLogin;
		private Action _onRegister;
		
		public void Enter(Action onSwitchToLogin, Action onRegister)
		{
			_Register(onSwitchToLogin, onRegister);
			_panel.SetActive(true);
		}
		
		public void Leave()
		{
			_Unregister();
			_panel.SetActive(false);
		}
		
		private void _Register(Action onSwitchToLogin, Action onRegister)
		{
			_onSwitchToLogin = onSwitchToLogin;
			_onRegister = onRegister;
			
			_switchToLoginButton.onClick.AddListener(_OnSwitchToLogin);
			_registerButton.onClick.AddListener(_OnRegister);
		}
		
		private void _Unregister()
		{
			_onSwitchToLogin = null;
			_onRegister = null;
			
			_switchToLoginButton.onClick.RemoveAllListeners();
			_registerButton.onClick.RemoveAllListeners();
		}
		
		private void _OnSwitchToLogin()
		{
			_onSwitchToLogin?.Invoke();
		}
		
		private void _OnRegister()
		{
			_onRegister?.Invoke();
		}
	}
}