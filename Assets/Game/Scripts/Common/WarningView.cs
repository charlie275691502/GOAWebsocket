using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
	public interface IWarningView
	{
		void Enter(string title, string content, Action onConfirm);
		void Leave();
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
		
		public void Enter(string title, string content, Action onConfirm)
		{
			_Enter(title, content);
			_Register(onConfirm);
			_panel.SetActive(true);
		}
		
		public void Leave()
		{
			_Unregister();
			_panel.SetActive(false);
			_Leave();
		}
		
		private void _Enter(string title, string content)
		{
			_title.text = title;
			_content.text = content;
		}
		
		private void _Leave()
		{
			_title.text = string.Empty;
			_content.text = string.Empty;
		}
		
		private void _Register(Action onConfirm)
		{
			_onConfirm = onConfirm;
			
			_confirmButton.onClick.AddListener(_OnConfirm);
		}
		
		private void _Unregister()
		{
			_onConfirm = null;
			
			_confirmButton.onClick.RemoveAllListeners();
		}
		
		private void _OnConfirm()
		{
			_onConfirm?.Invoke();
		}
	}
}