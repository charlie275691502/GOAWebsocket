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
		void Enter(RoomViewData viewData, Action switchToRoom);
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
		
		private Action _switchToRoom;
		
		public void Enter(RoomViewData viewData, Action switchToRoom)
		{
			_Enter(viewData);
			_Register(switchToRoom);
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
		
		private void _Register(Action switchToRoom)
		{
			_switchToRoom = switchToRoom;
			
			_button.onClick.AddListener(_SwitchToRoom);
		}
		
		private void _Unregister()
		{
			_switchToRoom = null;
			
			_button.onClick.RemoveAllListeners();
		}
		
		private void _SwitchToRoom()
		{
			_switchToRoom?.Invoke();
		}
	}
}
