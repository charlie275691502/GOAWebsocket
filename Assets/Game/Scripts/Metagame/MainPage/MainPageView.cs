using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.UI;

namespace Metagame
{
	public interface IMainPageView
	{
		void Enter(List<RoomViewData> viewDatas, Action<int> onJoinRoom, Action onCreateRoom);
		void Leave();
	}
	
	public class MainPageView : MonoBehaviour, IMainPageView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private EnhancedScroller _scroller;
		[SerializeField]
		private GameObject _prefab;
		[SerializeField]
		private Button _createRoomButton;
		
		private List<RoomViewData> _viewDatas;
		private SimpleEnhancedScrollerController<RoomListElementView, RoomViewData> _scrollerController;
		
		private Action<int> _onJoinRoom;
		private Action _onCreateRoom;
		
		[Zenject.Inject]
		public void Zenject()
		{
			_scrollerController = new SimpleEnhancedScrollerController<RoomListElementView, RoomViewData>();
			_scrollerController.Init(_scroller, _prefab);
			_scrollerController.OnInstantiateCell += _OnInstantiateCell;
		}
		
		public void Enter(List<RoomViewData> viewDatas, Action<int> onJoinRoom, Action onCreateRoom)
		{
			_Enter(viewDatas);
			_Register(onJoinRoom, onCreateRoom);
			_panel.SetActive(true);
		}
		
		public void Leave()
		{
			_Unregister();
			_panel.SetActive(false);
			_Leave();
		}
		
		private void _Enter(List<RoomViewData> viewDatas)
		{
			_viewDatas = viewDatas;
			
			_scrollerController.Display(viewDatas);
		}
		
		private void _Leave()
		{
			_scrollerController.Leave();
		}
		
		private void _Register(Action<int> onJoinRoom, Action onCreateRoom)
		{
			_onJoinRoom = onJoinRoom;
			_onCreateRoom = onCreateRoom;
			
			_createRoomButton.onClick.AddListener(_OnCreateRoom);
		}
		
		private void _Unregister()
		{
			_onJoinRoom = null;
			_onCreateRoom = null;
			
			_createRoomButton.onClick.RemoveAllListeners();
		}
		
		private void _OnInstantiateCell(RoomListElementView view, RoomViewData viewData, int dataIndex)
		{
			view.Init(() => _OnJoinRoom(viewData.Id));
		}
		
		private void _OnJoinRoom(int roomId)
		{
			_onJoinRoom?.Invoke(roomId);
		}
		
		private void _OnCreateRoom()
		{
			_onCreateRoom?.Invoke();
		}
	}
}
