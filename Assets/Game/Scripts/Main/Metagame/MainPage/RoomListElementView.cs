using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using Zenject;
using Cysharp.Threading.Tasks;
using System.Threading;
using Common.AssetSession;
using Common.Class;

namespace Metagame.MainPage
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
		
		private IAssetSession _assetSession;
		
#region EnhancedScrollerElement
		
		protected override void _Update()
		{
			
		}
		
		protected override void _Resolve(DiContainer container)
		{
			_assetSession = container.Resolve<IAssetSession>();
		}
		
		protected override void _Display(RoomViewData viewData)
		{
			_roomNameText.text = $"[{GameTypeUtility.GetAbbreviation(viewData.GameSetting.GameType)}] {viewData.RoomName}";
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
		
		protected override async UniTask _LoadAsset(RoomViewData viewData, CancellationTokenSource token)
		{
			for (int i=0; i < _playerInfoViews.Count; i++)
			{
				if (i < viewData.Players.Count) 
				{
					await _playerInfoViews[i].LoadAsset(viewData.Players[i], _assetSession, token);
				}
			}
			
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
