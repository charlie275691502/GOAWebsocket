using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Warning
{
	public interface IWarningView
	{
		void RegisterCallback(Action onConfirm);
		void Render(WarningProperty prop);
	}

	public class WarningView : MonoBehaviour, IWarningView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Text _title;
		[SerializeField]
		private Text _content;
		[SerializeField]
		private Button _confirmButton;
		
		private Action _onConfirm;

		private WarningProperty _prop;

		void IWarningView.RegisterCallback(Action onConfirm)
		{
			_onConfirm = onConfirm;

			_confirmButton.onClick.AddListener(_OnConfirm);
		}

		void IWarningView.Render(WarningProperty prop)
		{
			if (_prop == prop)
				return;
			_prop = prop;

			switch (prop.State)
			{
				case WarningState.Open:
					_Open();
					break;

				case WarningState.Idle:
				case WarningState.Confirm:
					_Render(prop);
					break;

				case WarningState.Close:
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
			_title.text = string.Empty;
			_content.text = string.Empty;
		}

		private void _Render(WarningProperty prop)
		{
			_title.text = prop.Title;
			_content.text = prop.Content;
		}

		private void _OnConfirm()
		{
			_onConfirm?.Invoke();
		}
	}
}