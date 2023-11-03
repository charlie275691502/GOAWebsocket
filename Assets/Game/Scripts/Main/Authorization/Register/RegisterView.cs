using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace Authorization.Register
{
	public interface IRegisterView
	{
		void RegisterCallback(Action<string, string, string, string> onRegister, Action onSwitchToLogin);
		void Render(RegisterProperty prop);
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
		
		private Action<string, string, string, string> _onRegister;
		private Action _onSwitchToLogin;
		
		private RegisterProperty _prop;
		
		void IRegisterView.RegisterCallback(Action<string, string, string, string> onRegister, Action onSwitchToLogin)
		{
			_onRegister = onRegister;
			_onSwitchToLogin = onSwitchToLogin;
			
			_registerButton.onClick.AddListener(_OnRegister);
			_switchToLoginButton.onClick.AddListener(_OnSwitchToLogin);
		}
		
		void IRegisterView.Render(RegisterProperty prop)
		{
			if (_prop == prop)
				return;
				
			switch (prop.State)
			{
				case RegisterState.Open:
					_Open();
					break;
					
				case RegisterState.Idle:
				case RegisterState.Register:
				case RegisterState.SwitchToLogin:
					_Render(prop);
					break;
					
				case RegisterState.Close:
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
		
		private void _Render(RegisterProperty prop)
		{
			
		}
		
		private void _OnRegister()
		{
			_onRegister?.Invoke(_usernameInputField.text, _passwordInputField.text, _confirmPasswordInputField.text, _emailInputField.text);
		}
				
		private void _OnSwitchToLogin()
		{
			_onSwitchToLogin?.Invoke();
		}
	}
}