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
		private List<RoomPlayerInfoView> _playerInfoViews;
		[SerializeField]
		private Button _sendMessageButton;
		[SerializeField]
		private InputField _messageInputField;
		
		[Header("Scroller")]
		[SerializeField]
		private EnhancedScrollerProxy _scroller;
		[SerializeField]
		private GameObject _prefab;
		[SerializeField]
		private string _scrollingAudioKey;
		[SerializeField]
		private float _lookAheadBefore;
		[SerializeField]
		private float _lookAheadAfter;
		
		private RoomWithMessagesViewData _viewData;
		private SimpleEnhancedScrollerController _scrollerController;
		private EnhancedScrollerDataModel<MessageListElementView, EnhancedScrollerElementViewData<MessageViewData>> _dataModel;
		
		private Action _onLeaveRoom;
		private Action<string> _onSendMessage;
		
		[Zenject.Inject]
		public void Zenject(DiContainer container)
		{
			_dataModel = new EnhancedScrollerDataModel<MessageListElementView, EnhancedScrollerElementViewData<MessageViewData>>(
				_prefab,
				_scroller.scrollDirection);
				
			_scrollerController = new SimpleEnhancedScrollerController(
				_scroller,
				_dataModel,
				_scrollingAudioKey,
				_lookAheadBefore,
				_lookAheadAfter,
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
			_dataModel.UpdateViewDatas(EnhancedScrollerUtility.GetViewDataList(_viewData.Messages));
			_scrollerController.Display(1);
		}
		
		public void AppendMessage(MessageViewData viewData)
		{
			_viewData.Messages.Add(viewData);
			_dataModel.UpdateViewDatas(EnhancedScrollerUtility.GetViewDataList(_viewData.Messages));
			_scrollerController.Display(1);
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
			_scrollerController.Clear();
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
			_roomNameText.text = $"[{GameTypeUtility.GetAbbreviation(_viewData.GameType)}] {_viewData.RoomName}";
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
