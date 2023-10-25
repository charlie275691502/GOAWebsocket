using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Authorization.Login
{
	public interface ILoginView
	{
		void RegisterCallback(Action<string, string> onLogin, Action onSwitchToRegister);
		void Render(LoginProperty prop);
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
		
		private Action<string, string> _onLogin;
		private Action _onSwitchToRegister;

		private LoginProperty _prop;

		void ILoginView.RegisterCallback(Action<string, string> onLogin, Action onSwitchToRegister)
		{
			_onLogin = onLogin;
			_onSwitchToRegister = onSwitchToRegister;

			_loginButton.onClick.AddListener(_OnLogin);
			_switchToRegisterButton.onClick.AddListener(_OnSwitchToRegister);
		}

		void ILoginView.Render(LoginProperty prop)
		{
			if (_prop == prop)
				return;

			switch (prop.State)
			{
				case LoginState.Open:
					_Open();
					break;

				case LoginState.Idle:
				case LoginState.Login:
				case LoginState.SwitchToRegister:
					_Render(prop);
					break;

				case LoginState.Close:
					_Close();
					break;

				default:
					break;
			}
		}

		private void _Open()
		{
			_panel.SetActive(true);
		}

		private void _Close()
		{
			_panel.SetActive(false);
		}

		private void _Render(LoginProperty prop)
		{

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