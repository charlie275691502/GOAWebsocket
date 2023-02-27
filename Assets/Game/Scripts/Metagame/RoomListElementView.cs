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
		void Enter(RoomViewData viewData);
		void Leave();
	}
	
	public class RoomListElementView : MonoBehaviour, IRoomListElementView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Text _roomNameText;
		[SerializeField]
		private List<RoomListElementPlayerInfoView> _playerInfoViews;
		
		public void Enter(RoomViewData viewData)
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
			
			_Register();
			_panel.SetActive(true);
		}
		
		public void Leave()
		{
			_playerInfoViews.ForEach(view => view.Leave());
			_Unregister();
			_panel.SetActive(false);
		}
		
		private void _Register()
		{
		}
		
		private void _Unregister()
		{
		}
	}
}
