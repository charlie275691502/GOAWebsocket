using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Authorization
{
	public interface ILoginView
	{
		Action OnClickSwitchToRegister { get; set; }
		Action OnClickLogin { get; set; }
		void Enter();
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
		
		public Action OnClickSwitchToRegister { get; set; }
		public Action OnClickLogin { get; set; }
		
		public void Enter()
		{
			_Register();
			_panel.SetActive(true);
		}
		
		public void Leave()
		{
			_Unregister();
			_panel.SetActive(false);
		}
		
		private void _Register()
		{
			_switchToRegisterButton.onClick.AddListener(_OnClickSwitchToRegister);
			_loginButton.onClick.AddListener(_OnClickLogin);
		}
		
		private void _Unregister()
		{
			_switchToRegisterButton.onClick.RemoveAllListeners();
			_loginButton.onClick.RemoveAllListeners();
		}
		
		private void _OnClickSwitchToRegister()
		{
			OnClickSwitchToRegister?.Invoke();
		}
		
		private void _OnClickLogin()
		{
			OnClickLogin?.Invoke();
		}
	}
}