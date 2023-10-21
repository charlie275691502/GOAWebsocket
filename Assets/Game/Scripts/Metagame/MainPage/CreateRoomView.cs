using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace Metagame
{
	public interface ICreateRoomView
	{
		void Enter(Action<string, GameType , int> onConfirm, Action onCancel);
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
		[SerializeField]
		private Dropdown _gameTypeDropdown;
		[SerializeField]
		private InputField _playerPlotInputField;
		
		[SerializeField]
		private DropDownOptionPair[] _dropdownOptions;
		[Serializable]
		public struct DropDownOptionPair
		{
			public string Name;
			public GameType GameType;
		}
		
		private GameType[] _dropdownGameTypes;
		private Action<string, GameType , int> _onConfirm;
		private Action _onCancel;
		
		private void Start()
		{
			_gameTypeDropdown.options = _dropdownOptions
				.Select(option => new Dropdown.OptionData(option.Name))
				.ToList();
				
			_dropdownGameTypes = _dropdownOptions
				.Select(option => option.GameType)
				.ToArray();
		}
		
		public void Enter(Action<string, GameType , int> onConfirm, Action onCancel)
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
			_gameTypeDropdown.value = 0;
			_playerPlotInputField.text = "2";
		}
		
		private void _Leave()
		{
		}
		
		private void _Register(Action<string, GameType , int> onConfirm, Action onCancel)
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
			_onConfirm?.Invoke(
				_roomNameInputField.text,
				_dropdownGameTypes[_gameTypeDropdown.value],
				int.TryParse(_playerPlotInputField.text, out var playerPlot)
					? playerPlot
					: 2);
		}
		
		private void _OnCancel()
		{
			_onCancel?.Invoke();
		}
	}
}
