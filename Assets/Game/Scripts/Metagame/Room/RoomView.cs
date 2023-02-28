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
		void Enter(RoomWithMessagesViewData viewData, Action switchToMainPage, Action<string> onSendMessage);
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
		private Action _switchToMainPage;
		private Action<string> _onSendMessage;
		
		public void Enter(RoomWithMessagesViewData viewData, Action switchToMainPage, Action<string> onSendMessage)
		{
			_Enter(viewData);
			_Register(switchToMainPage, onSendMessage);
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
			_roomNameText.text = viewData.RoomName;
			for (int i=0; i < _playerInfoViews.Count; i++)
			{
				if (i < viewData.Players.Count) 
				{
					_playerInfoViews[i].Enter(viewData.Players[i]);
				} else 
				{
					_playerInfoViews[i].EnterEmpty();
				}
			}
			
			_scrollRect.Enter(_pool, _OnInstantiateMessageElement);
			_scrollRect.FillItems(viewData.Messages.Count);
		}
		
		private void _Leave()
		{
			_roomNameText.text = string.Empty;
			_playerInfoViews.ForEach(view => view.Leave());
			_scrollRect.Leave();
		}
		
		private void _Register(Action switchToMainPage, Action<string> onSendMessage)
		{
			_switchToMainPage = switchToMainPage;
			_onSendMessage = onSendMessage;
			
			_backButton.onClick.AddListener(_SwitchToMainPage);
			_sendMessageButton.onClick.AddListener(_OnSendMessage);
		}
		
		private void _Unregister()
		{
			_switchToMainPage = null;
			
			_backButton.onClick.RemoveAllListeners();
			_sendMessageButton.onClick.RemoveAllListeners();
		}
		
		private void _SwitchToMainPage()
		{
			_switchToMainPage?.Invoke();
		}
		
		private void _OnSendMessage()
		{
			_onSendMessage?.Invoke(_messageInputField.text);
		}
		
		private void _OnInstantiateMessageElement(int index, IMessageListElementView view)
		{
			view.Enter(_viewData.Messages[index]);
		}
	}
}
