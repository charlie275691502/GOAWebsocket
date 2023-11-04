using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Metagame.Room
{
	public interface IRoomView
	{
		void RegisterCallback(Action onLeaveRoom, Action<string> onSendMessage);
		void Render(RoomProperty prop);
	}
	
	public class RoomView : MonoBehaviour, IRoomView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Button _backButton;
		[SerializeField]
		private Text _roomNameText;
		[SerializeField]
		private Button _sendMessageButton;
		[SerializeField]
		private InputField _messageInputField;
		
		[Header("MessageScroller")]
		[SerializeField]
		private EnhancedScrollerProxy _messageScroller;
		[SerializeField]
		private GameObject _messagePrefab;
		[SerializeField]
		private string _messageScrollingAudioKey;
		[SerializeField]
		private float _messageLookAheadBefore;
		[SerializeField]
		private float _messageLookAheadAfter;
		
		[Header("PlayerScroller")]
		[SerializeField]
		private EnhancedScrollerProxy _playerScroller;
		[SerializeField]
		private GameObject _playerPrefab;
		[SerializeField]
		private string _playerScrollingAudioKey;
		[SerializeField]
		private float _playerLookAheadBefore;
		[SerializeField]
		private float _playerLookAheadAfter;
		
		private SimpleEnhancedScrollerController _messageScrollerController;
		private EnhancedScrollerDataModel<MessageListElementView, EnhancedScrollerElementViewData<MessageViewData>> _messageDataModel;
		private SimpleEnhancedScrollerController _playerScrollerController;
		private EnhancedScrollerDataModel<PlayerListElementView, EnhancedScrollerElementViewData<PlayerData>> _playerDataModel;
		
		private Action _onLeaveRoom;
		private Action<string> _onSendMessage;
		
		[Zenject.Inject]
		public void Zenject(DiContainer container)
		{
			_messageDataModel = new EnhancedScrollerDataModel<MessageListElementView, EnhancedScrollerElementViewData<MessageViewData>>(
				_messagePrefab,
				_messageScroller.scrollDirection);
			_playerDataModel = new EnhancedScrollerDataModel<PlayerListElementView, EnhancedScrollerElementViewData<PlayerData>>(
				_playerPrefab,
				_playerScroller.scrollDirection);
				
			_messageScrollerController = new SimpleEnhancedScrollerController(
				_messageScroller,
				_messageDataModel,
				_messageScrollingAudioKey,
				_messageLookAheadBefore,
				_messageLookAheadAfter,
				container);
			_playerScrollerController = new SimpleEnhancedScrollerController(
				_playerScroller,
				_playerDataModel,
				_playerScrollingAudioKey,
				_playerLookAheadBefore,
				_playerLookAheadAfter,
				container);
		}

		private RoomProperty _prop;

		void IRoomView.RegisterCallback(Action onLeaveRoom, Action<string> onSendMessage)
		{
			_onLeaveRoom = onLeaveRoom;
			_onSendMessage = onSendMessage;
			
			_backButton.onClick.AddListener(_SwitchToMainPage);
			_sendMessageButton.onClick.AddListener(_OnSendMessage);
		}

		void IRoomView.Render(RoomProperty prop)
		{
			if (_prop == prop)
				return;
			_prop = prop;

			switch (prop.State)
			{
				case RoomState.Open:
					_Open();
					break;

				case RoomState.Idle:
				case RoomState.SendMessage:
				case RoomState.Leave:
					_Render(prop);
					break;

				case RoomState.Close:
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
			_roomNameText.text = string.Empty;
			_messageScrollerController.Clear();
			_playerScrollerController.Clear();
		}

		private void _Render(RoomProperty prop)
		{
			_roomNameText.text = $"[{GameTypeUtility.GetAbbreviation(prop.Room.GameSetting.GameType)}] {prop.Room.RoomName}";
			_playerDataModel.UpdateViewDatas(EnhancedScrollerUtility.GetViewDataList(prop.Room.Players, prop.Room.GameSetting.PlayerPlot));
			_playerScrollerController.Display();
			_messageDataModel.UpdateViewDatas(EnhancedScrollerUtility.GetViewDataList(prop.Room.Messages));
			_messageScrollerController.Display(1);
		}
		
		private void _SwitchToMainPage()
		{
			_onLeaveRoom?.Invoke();
		}
		
		private void _OnSendMessage()
		{
			_onSendMessage?.Invoke(_messageInputField.text);
			_messageInputField.text = string.Empty;
		}
	}
}
