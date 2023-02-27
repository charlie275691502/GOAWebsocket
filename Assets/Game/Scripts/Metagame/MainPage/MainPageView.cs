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
		void Enter(List<RoomViewData> viewDatas);
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
		
		public void Enter(List<RoomViewData> viewDatas)
		{
			_viewDatas = viewDatas;
			
			_scrollRect.Enter(_pool, _OnInstantiateRoomListElement);
			_scrollRect.FillItems(viewDatas.Count);
			
			_Register();
			_panel.SetActive(true);
		}
		
		public void Leave()
		{
			_Unregister();
			_panel.SetActive(false);
			_scrollRect.Leave();
		}
		
		private void _Register()
		{
		}
		
		private void _Unregister()
		{
		}
		
		private void _OnInstantiateRoomListElement(int index, IRoomListElementView view)
		{
			view.Enter(_viewDatas[index]);
		}
	}
}
