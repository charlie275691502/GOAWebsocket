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
		
		public void Enter(List<RoomViewData> viewDatas)
		{
			_Register();
			_panel.SetActive(true);
		}
		
		public void Leave()
		{
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