using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace Authorization
{
	public interface IRegisterView
	{
		void Enter(Action onSwitchToLogin, Action<string, string, string, string> onRegister);
		void Leave();
	}
	
	public class RegisterView : MonoBehaviour, IRegisterView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private InputField _usernameInputField;
		[SerializeField]
		private InputField _passwordInputField;
		[SerializeField]
		private InputField _confirmPasswordInputField;
		[SerializeField]
		private InputField _emailInputField;
		[SerializeField]
		private Button _switchToLoginButton;
		[SerializeField]
		private Button _registerButton;
		
		private Action _onSwitchToLogin;
		private Action<string, string, string, string> _onRegister;
		
		public void Enter(Action onSwitchToLogin, Action<string, string, string, string> onRegister)
		{
			_Register(onSwitchToLogin, onRegister);
			_panel.SetActive(true);
		}
		
		public void Leave()
		{
			_Unregister();
			_panel.SetActive(false);
		}
		
		private void _Register(Action onSwitchToLogin, Action<string, string, string, string> onRegister)
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
			_onRegister?.Invoke(_usernameInputField.text, _passwordInputField.text, _confirmPasswordInputField.text, _emailInputField.text);
		}
	}
}