using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace Metagame
{
	public interface ICreateRoomView
	{
		void Enter(Action<string> onConfirm, Action onCancel);
		void Leave();
	}
	
	public class CreateRoomView : MonoBehaviour, ICreateRoomView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Button _confirmButton;
		[SerializeField]
		private Button _cancelButton;
		[SerializeField]
		private InputField _roomNameInputField;
		
		private Action<string> _onConfirm;
		private Action _onCancel;
		
		public void Enter(Action<string> onConfirm, Action onCancel)
		{
			_Enter();
			_Register(onConfirm, onCancel);
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
			_roomNameInputField.text = string.Empty;
		}
		
		private void _Leave()
		{
		}
		
		private void _Register(Action<string> onConfirm, Action onCancel)
		{
			_onConfirm = onConfirm;
			_onCancel = onCancel;
			
			_confirmButton.onClick.AddListener(_OnConfirm);
			_cancelButton.onClick.AddListener(_OnCancel);
		}
		
		private void _Unregister()
		{
			_onConfirm = null;
			_onCancel = null;
			
			_confirmButton.onClick.RemoveAllListeners();
			_cancelButton.onClick.RemoveAllListeners();
		}
		
		private void _OnConfirm()
		{
			_onConfirm?.Invoke(_roomNameInputField.text);
		}
		
		private void _OnCancel()
		{
			_onCancel?.Invoke();
		}
	}
}
