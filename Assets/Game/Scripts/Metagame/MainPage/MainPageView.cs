using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Metagame.MainPage
{
	public interface IMainPageView
	{
		void RegisterCallback(Action<int> onJoinRoom, Action onCreateRoom);
		void Render(MainPageProperty prop);
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
		private MainPageProperty _prop;

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

		void IMainPageView.RegisterCallback(Action<int> onJoinRoom, Action onCreateRoom)
		{
			_onJoinRoom = onJoinRoom;
			_onCreateRoom = onCreateRoom;

			_createRoomButton.onClick.AddListener(_OnCreateRoom);
		}

		void IMainPageView.Render(MainPageProperty prop)
		{
			if (_prop == prop)
				return;

			switch (prop.State)
			{
				case MainPageState.Open:
					_Open();
					break;

				case MainPageState.Idle:
				case MainPageState.JoinRoom:
				case MainPageState.CreateRoom:
					_Render(prop);
					break;

				case MainPageState.Close:
					_Close();
					break;

				default:
					break;
			}
		}

		public void _Open()
		{
			_panel.SetActive(true);
		}
		
		public void _Close()
		{
			_panel.SetActive(false);
			_scrollerController.Clear();
		}

		private void _Render(MainPageProperty prop)
		{
			_dataModel.UpdateViewDatas(EnhancedScrollerUtility.GetViewDataList(prop.Rooms));
			_scrollerController.Display();
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
