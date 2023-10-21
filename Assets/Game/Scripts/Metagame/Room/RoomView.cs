using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Metagame
{
	public interface IRoomView
	{
		void Enter(RoomWithMessagesViewData viewData, Action onLeaveRoom, Action<string> onSendMessage);
		void AppendMessage(MessageViewData viewData);
		void UpdateRoom(RoomViewData viewData);
		void Leave();
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
		
		private RoomWithMessagesViewData _viewData;
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
		
		public void Enter(RoomWithMessagesViewData viewData, Action onLeaveRoom, Action<string> onSendMessage)
		{
			_Enter(viewData);
			_Register(onLeaveRoom, onSendMessage);
			_panel.SetActive(true);
		}
		
		public void Leave()
		{
			_Unregister();
			_panel.SetActive(false);
			_Leave();
		}
		
		private void _Enter(RoomWithMessagesViewData viewData)
		{
			_viewData = viewData;
			
			_UpdateRoom();
			_messageDataModel.UpdateViewDatas(EnhancedScrollerUtility.GetViewDataList(_viewData.Messages));
			_messageScrollerController.Display(1);
		}
		
		public void AppendMessage(MessageViewData viewData)
		{
			_viewData.Messages.Add(viewData);
			_messageDataModel.UpdateViewDatas(EnhancedScrollerUtility.GetViewDataList(_viewData.Messages));
			_messageScrollerController.Display(1);
		}
		public void UpdateRoom(RoomViewData viewData)
		{
			_viewData.Id = viewData.Id;
			_viewData.RoomName = viewData.RoomName;
			_viewData.Players = viewData.Players;
			_UpdateRoom();
		}
		
		private void _Leave()
		{
			_roomNameText.text = string.Empty;
			_messageScrollerController.Clear();
			_playerScrollerController.Clear();
		}
		
		private void _Register(Action onLeaveRoom, Action<string> onSendMessage)
		{
			_onLeaveRoom = onLeaveRoom;
			_onSendMessage = onSendMessage;
			
			_backButton.onClick.AddListener(_SwitchToMainPage);
			_sendMessageButton.onClick.AddListener(_OnSendMessage);
		}
		
		private void _Unregister()
		{
			_onLeaveRoom = null;
			
			_backButton.onClick.RemoveAllListeners();
			_sendMessageButton.onClick.RemoveAllListeners();
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
		
		private void _UpdateRoom()
		{
			_roomNameText.text = $"[{GameTypeUtility.GetAbbreviation(_viewData.GameSetting.GameType)}] {_viewData.RoomName}";
			_playerDataModel.UpdateViewDatas(EnhancedScrollerUtility.GetViewDataList(_viewData.Players, _viewData.GameSetting.PlayerPlot));
			_playerScrollerController.Display();
		}
	}
}
