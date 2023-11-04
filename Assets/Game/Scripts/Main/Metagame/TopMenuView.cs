using UnityEngine;
using UnityEngine.UI;
using Common;

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
			_Enter(data);
			_Register();
			_panel.SetActive(true);
		}
		
		public void Leave()
		{
			_Unregister();
			_panel.SetActive(false);
			_Leave();
		}
		
		private void _Enter(BackendPlayerData data)
		{
			_nickNameText.text = data.PlayerData.NickName;
			_coinText.text = data.PlayerData.Coin.ToString();
		}
		
		private void _Leave()
		{
			_nickNameText.text = string.Empty;
			_coinText.text = string.Empty;
		}
		
		private void _Register()
		{
		}
		
		private void _Unregister()
		{
		}
		
		public void UpdateNickName(string nickName)
		{
			_nickNameText.text = nickName;
		}
		
		public void UpdateCoin(int coin)
		{
			_coinText.text = coin.ToString();
		}
	}
}