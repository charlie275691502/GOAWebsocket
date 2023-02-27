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
			_Enter(data);
			_Register();
			_panel.SetActive(true);
		}
		
		public void EnterEmpty()
		{
			_EnterEmpty();
			_Register();
			_panel.SetActive(true);
		}
		
		public void Leave()
		{
			_Unregister();
			_panel.SetActive(false);
			_Leave();
		}
		
		private void _Enter(PlayerData data)
		{
			_nickNameText.text = data.NickName;
		}
		
		private void _EnterEmpty()
		{
			_nickNameText.text = _emptyNickName;
		}
		
		private void _Leave()
		{
			_nickNameText.text = string.Empty;
		}
		
		private void _Register()
		{
		}
		
		private void _Unregister()
		{
		}
	}
}
