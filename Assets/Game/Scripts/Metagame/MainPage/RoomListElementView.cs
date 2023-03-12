using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace Metagame
{
	public interface IRoomListElementView
	{
		void Enter(RoomViewData viewData, Action onJoinRoom);
		void Leave();
	}
	
	public class RoomListElementView : MonoBehaviour, IRoomListElementView
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
		
		public void Enter(RoomViewData viewData, Action onJoinRoom)
		{
			_Enter(viewData);
			_Register(onJoinRoom);
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
