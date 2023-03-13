using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

namespace Metagame
{
	public class RoomListElementView : EnhancedScrollerCellView, ISimpleEnhancedScrollerElement<RoomViewData>
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Button _button;
		[SerializeField]
		private Text _roomNameText;
		[SerializeField]
		private List<RoomListElementPlayerInfoView> _playerInfoViews;
		
		private Action _onJoinRoom;
		
		public void Init(Action onJoinRoom)
		{
			_Register(onJoinRoom);
		}
		
		public void Enter(RoomViewData viewData)
		{
			_Enter(viewData);
			_panel.SetActive(true);
		}
		
		public void Leave()
		{
			_Unregister();
			_panel.SetActive(false);
			_Leave();
		}
		
		private void _Enter(RoomViewData viewData)
		{
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
		}
		
		private void _Leave()
		{
			_playerInfoViews.ForEach(view => view.Leave());
		}
		
		private void _Register(Action onJoinRoom)
		{
			_onJoinRoom = onJoinRoom;
			
			_button.onClick.AddListener(_OnJoinRoom);
		}
		
		private void _Unregister()
		{
			_onJoinRoom = null;
			
			_button.onClick.RemoveAllListeners();
		}
		
		private void _OnJoinRoom()
		{
			_onJoinRoom?.Invoke();
		}
	}
}
