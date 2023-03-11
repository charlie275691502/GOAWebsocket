using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace Metagame
{
	public interface IMainPageView
	{
		void Enter(List<RoomViewData> viewDatas, Action<int> onJoinRoom);
		void Leave();
	}
	
	public class MainPageView : MonoBehaviour, IMainPageView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private GameObjectPool _pool;
		[SerializeField]
		private RoomListDynamicScrollRect _scrollRect;
		
		private List<RoomViewData> _viewDatas;
		private Action<int> _onJoinRoom;
		
		public void Enter(List<RoomViewData> viewDatas, Action<int> onJoinRoom)
		{
			_Enter(viewDatas);
			_Register(onJoinRoom);
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
			
			_scrollRect.Enter(_pool, _OnInstantiateRoomListElement);
			_scrollRect.FillItems(viewDatas.Count);
		}
		
		private void _Leave()
		{
			_scrollRect.Leave();
		}
		
		private void _Register(Action<int> onJoinRoom)
		{
			_onJoinRoom = onJoinRoom;
		}
		
		private void _Unregister()
		{
			_onJoinRoom = null;
		}
		
		private void _SwitchToRoom(int roomId)
		{
			_onJoinRoom?.Invoke(roomId);
		}
		
		private void _OnInstantiateRoomListElement(int index, IRoomListElementView view)
		{
			view.Enter(_viewDatas[index], () => _SwitchToRoom(_viewDatas[index].Id));
		}
	}
}
