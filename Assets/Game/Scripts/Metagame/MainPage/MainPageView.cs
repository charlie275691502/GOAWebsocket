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
		void Enter(List<RoomViewData> viewDatas, Action<int> onJoinRoom, Action onCreateRoom);
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
		[SerializeField]
		private Button _createRoomButton;
		
		private List<RoomViewData> _viewDatas;
		private Action<int> _onJoinRoom;
		private Action _onCreateRoom;
		
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
			
			_scrollRect.Enter(_pool, _OnInstantiateRoomListElement);
			_scrollRect.FillItems(viewDatas.Count);
		}
		
		private void _Leave()
		{
			_scrollRect.Leave();
		}
		
		private void _Register(Action<int> onJoinRoom, Action onCreateRoom)
		{
			_onJoinRoom = onJoinRoom;
			_onCreateRoom = onCreateRoom;
			
			_createRoomButton.onClick.AddListener(_OnCretaeRoom);
		}
		
		private void _Unregister()
		{
			_onJoinRoom = null;
			_onCreateRoom = null;
			
			_createRoomButton.onClick.RemoveAllListeners();
		}
		
		private void _OnJoinRoom(int roomId)
		{
			_onJoinRoom?.Invoke(roomId);
		}
		
		private void _OnCretaeRoom()
		{
			_onCreateRoom?.Invoke();
		}
		
		private void _OnInstantiateRoomListElement(int index, IRoomListElementView view)
		{
			view.Enter(_viewDatas[index], () => _OnJoinRoom(_viewDatas[index].Id));
		}
	}
}
