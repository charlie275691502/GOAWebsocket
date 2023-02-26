using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;
using Web;

namespace Metagame
{
	public interface ITopMenuView
	{
		void Enter(BackendPlayerData data);
		void Leave();
	}
	
	public class TopMenuView : MonoBehaviour, IBackendPlayerView, ITopMenuView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Text _nickNameText;
		[SerializeField]
		private Text _coinText;
		
		public void Enter(BackendPlayerData data)
		{
			_nickNameText.text = data.NickName;
			_coinText.text = data.Coin.ToString();
			_panel.SetActive(true);
		}
		
		public void UpdateNickName(string nickName)
		{
			_nickNameText.text = nickName;
		}
		
		public void UpdateCoin(int coin)
		{
			_coinText.text = coin.ToString();
		}
		
		public void Leave()
		{
			_panel.SetActive(false);
		}
	}
}