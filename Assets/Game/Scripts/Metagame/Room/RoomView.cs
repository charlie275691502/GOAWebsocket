using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.UI;

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
		private List<RoomPlayerInfoView> _playerInfoViews;
		[SerializeField]
		private GameObjectPool _pool;
		[SerializeField]
		private RoomMessageDynamicScrollRect _scrollRect;
		[SerializeField]
		private Button _sendMessageButton;
		[SerializeField]
		private InputField _messageInputField;
		
		private RoomWithMessagesViewData _viewData;
		private Action _onLeaveRoom;
		private Action<string> _onSendMessage;
		
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
			_scrollRect.Enter(_pool, _OnInstantiateMessageElement);
			_scrollRect.FillItems(viewData.Messages.Count);
		}
		
		public void AppendMessage(MessageViewData viewData)
		{
			_viewData.Messages.Add(viewData);
			_scrollRect.AppendItem(1);
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
			_playerInfoViews.ForEach(view => view.Leave());
			_scrollRect.Leave();
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
		
		private void _OnInstantiateMessageElement(int index, IMessageListElementView view)
		{
			view.Enter(_viewData.Messages[index]);
		}
		
		private void _UpdateRoom()
		{
			_roomNameText.text = _viewData.RoomName;
			for (int i=0; i < _playerInfoViews.Count; i++)
			{
				if (i < _viewData.Players.Count) 
				{
					_playerInfoViews[i].Enter(_viewData.Players[i]);
				} else 
				{
					_playerInfoViews[i].EnterEmpty();
				}
			}
		}
	}
}
