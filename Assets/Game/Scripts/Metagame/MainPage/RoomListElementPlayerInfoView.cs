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
		[SerializeField]
		private Text _nickNameText;
		[SerializeField]
		private Image _avatarImage;
		[SerializeField]
		private string _emptyNickName;
		
		public void Enter(PlayerData data)
		{
			_nickNameText.text = data.NickName;
			_Enter();
		}
		
		public void EnterEmpty()
		{
			_nickNameText.text = _emptyNickName;
			_Enter();
		}
		
		private void _Enter()
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
