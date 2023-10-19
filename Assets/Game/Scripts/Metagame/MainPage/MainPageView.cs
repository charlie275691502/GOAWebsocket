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
		private Button _createRoomButton;
		
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
		
		private SimpleEnhancedScrollerController _scrollerController;
		private EnhancedScrollerDataModel<RoomListElementView, EnhancedScrollerElementViewData<RoomViewData>> _dataModel;
		
		private Action<int> _onJoinRoom;
		private Action _onCreateRoom;
		
		[Zenject.Inject]
		public void Zenject(DiContainer container)
		{
			_dataModel = new EnhancedScrollerDataModel<RoomListElementView, EnhancedScrollerElementViewData<RoomViewData>>(
				_prefab,
				_scroller.scrollDirection);
			_dataModel.OnInstantiateCell += _OnInstantiateCell;
				
			_scrollerController = new SimpleEnhancedScrollerController(
				_scroller,
				_dataModel,
				_scrollingAudioKey,
				_lookAheadBefore,
				_lookAheadAfter,
				container);
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
			_dataModel.UpdateViewDatas(EnhancedScrollerUtility.GetViewDataList(viewDatas));
			_scrollerController.Display();
		}
		
		private void _Leave()
		{
			_scrollerController.Clear();
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
		
		private void _OnInstantiateCell(RoomListElementView view, EnhancedScrollerElementViewData<RoomViewData> viewData, int dataIndex)
		{
			view.OnJoinRoom += () => _OnJoinRoom(viewData.Data.Id);
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
