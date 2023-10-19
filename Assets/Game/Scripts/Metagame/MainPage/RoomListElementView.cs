using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using Zenject;

namespace Metagame
{
	public class RoomListElementView : SimpleEnhancedScrollerElement<RoomViewData>
	{
		[SerializeField]
		private Button _button;
		[SerializeField]
		private Text _roomNameText;
		[SerializeField]
		private List<RoomListElementPlayerInfoView> _playerInfoViews;
		
		public Action OnJoinRoom;
		
#region EnhancedScrollerElement
		
		protected override void _Update()
		{
			
		}
		
		protected override void _Resolve(DiContainer container)
		{
			
		}
		
		protected override void _Display(RoomViewData viewData)
		{
			_roomNameText.text = $"[{GameTypeUtility.GetAbbreviation(viewData.GameType)}] {viewData.RoomName}";
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
		
		protected override void _DisplayEmpty()
		{
			_roomNameText.text = string.Empty;
			_playerInfoViews.ForEach(view => view.EnterEmpty());
		}
		
		protected override void _Refresh(RoomViewData viewData)
		{
			
		}
		
		protected override IEnumerator _LoadAsset(RoomViewData viewData)
		{
			yield break;
		}
		
		protected override void _Leave()
		{
			_playerInfoViews.ForEach(view => view.Leave());
		}
		
		protected override void _Register()
		{
			_button.onClick.AddListener(_OnJoinRoom);
		}
		
		protected override void _Unregister()
		{
			OnJoinRoom = null;
			_button.onClick.RemoveAllListeners();
		}
		
#endregion
		
		private void _OnJoinRoom()
		{
			OnJoinRoom?.Invoke();
		}
	}
}
