using System;
using UnityEngine;
using UnityEngine.UI;

namespace Authorization
{
	public interface IAuthorizationView
	{
		void RegisterCallback(Action onClickLogin);
		void Render(AuthorizationProperty prop);
	}

	public class AuthorizationView : MonoBehaviour, IAuthorizationView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Button _button;

		private Action _onClickLogin;

		private AuthorizationProperty _prop;

		void IAuthorizationView.RegisterCallback(Action onClickLogin)
		{
			_onClickLogin = onClickLogin;

			_button.onClick.AddListener(_OnClickLogin);
		}

		void IAuthorizationView.Render(AuthorizationProperty prop)
		{
			if (_prop == prop)
				return;
			_prop = prop;
			
			switch (prop.State)
			{
				case AuthorizationState.Open:
					_Open();
					break;

				case AuthorizationState.Idle:
				case AuthorizationState.Login:
				case AuthorizationState.Register:
					_Render(prop);
					break;

				case AuthorizationState.Close:
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

		private void _Render(AuthorizationProperty prop)
		{

		}

		private void _OnClickLogin()
		{
			_onClickLogin?.Invoke();
		}
	}
}
