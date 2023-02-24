using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Authorization
{
	public interface IRegisterView
	{
		Action OnClickSwitchToLogin { get; set; }
		Action OnClickRegister { get; set; }
		void Enter();
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
		
		public Action OnClickSwitchToLogin { get; set; }
		public Action OnClickRegister { get; set; }
		
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
			_switchToLoginButton.onClick.AddListener(_OnClickSwitchToLogin);
			_registerButton.onClick.AddListener(_OnClickRegister);
		}
		
		private void _Unregister()
		{
			_switchToLoginButton.onClick.RemoveAllListeners();
			_registerButton.onClick.RemoveAllListeners();
		}
		
		private void _OnClickSwitchToLogin()
		{
			OnClickSwitchToLogin?.Invoke();
		}
		
		private void _OnClickRegister()
		{
			OnClickRegister?.Invoke();
		}
	}
}