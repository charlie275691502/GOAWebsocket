using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace Metagame
{
	public class RoomListElementPlayerInfoView : MonoBehaviour
	{
		[SerializeField]
		private GameObject _panel;
		
		public void Enter()
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
