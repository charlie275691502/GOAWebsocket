using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Authorization
{
	public interface ILoginView
	{
		void Enter(Action onSwitchToRegister, Action<string, string> onLogin);
		void Leave();
	}
	
	public class LoginView : MonoBehaviour, ILoginView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private InputField _usernameInputField;
		[SerializeField]
		private InputField _passwordInputField;
		[SerializeField]
		private Button _switchToRegisterButton;
		[SerializeField]
		private Button _loginButton;
		
		private Action _onSwitchToRegister;
		private Action<string, string> _onLogin;
		
		public void Enter(Action onSwitchToRegister, Action<string, string> onLogin)
		{
			_Enter();
			_Register(onSwitchToRegister, onLogin);
			_panel.SetActive(true);
		}
		
		public void Leave()
		{
			_Unregister();
			_panel.SetActive(false);
			_Leave();
		}
		
		private void _Enter()
		{
		}
		
		private void _Leave()
		{
		}
		
		private void _Register(Action onSwitchToRegister, Action<string, string> onLogin)
		{
			_onSwitchToRegister = onSwitchToRegister;
			_onLogin = onLogin;
			
			_switchToRegisterButton.onClick.AddListener(_OnSwitchToRegister);
			_loginButton.onClick.AddListener(_OnLogin);
		}
		
		private void _Unregister()
		{
			_onSwitchToRegister = null;
			_onLogin = null;
			
			_switchToRegisterButton.onClick.RemoveAllListeners();
			_loginButton.onClick.RemoveAllListeners();
		}
		
		private void _OnSwitchToRegister()
		{
			_onSwitchToRegister?.Invoke();
		}
		
		private void _OnLogin()
		{
			_onLogin?.Invoke(_usernameInputField.text, _passwordInputField.text);
		}
	}
}