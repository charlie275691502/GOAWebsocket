using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace Metagame.MainPage.CreateRoom
{
	public interface ICreateRoomView
	{
		void RegisterCallback(Action<string, GameType, int> onConfirm, Action onCancel);
		void Render(CreateRoomProperty prop);
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
		private CreateRoomProperty _prop;

		private void Start()
		{
			_gameTypeDropdown.options = _dropdownOptions
				.Select(option => new Dropdown.OptionData(option.Name))
				.ToList();
				
			_dropdownGameTypes = _dropdownOptions
				.Select(option => option.GameType)
				.ToArray();
		}

		void ICreateRoomView.RegisterCallback(Action<string, GameType, int> onConfirm, Action onCancel)
		{
			_onConfirm = onConfirm;
			_onCancel = onCancel;

			_confirmButton.onClick.AddListener(_OnConfirm);
			_cancelButton.onClick.AddListener(_OnCancel);
		}

		void ICreateRoomView.Render(CreateRoomProperty prop)
		{
			if (_prop == prop)
				return;
			_prop = prop;

			switch (prop.State)
			{
				case CreateRoomState.Open:
					_Open();
					break;

				case CreateRoomState.Idle:
				case CreateRoomState.Confirm:
				case CreateRoomState.Cancel:
					_Render(prop);
					break;

				case CreateRoomState.Close:
					_Close();
					break;

				default:
					break;
			}
		}

		private void _Open()
		{
			_panel.SetActive(true);
			_roomNameInputField.text = string.Empty;
			_gameTypeDropdown.value = 0;
			_playerPlotInputField.text = "2";
		}

		private void _Close()
		{
			_panel.SetActive(false);
		}

		private void _Render(CreateRoomProperty prop)
		{
		}
		
		private void _Register(Action<string, GameType , int> onConfirm, Action onCancel)
		{
			_onConfirm = onConfirm;
			_onCancel = onCancel;
			
			_confirmButton.onClick.AddListener(_OnConfirm);
			_cancelButton.onClick.AddListener(_OnCancel);
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
