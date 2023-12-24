using System.Threading;
using Common;
using Common.AssetSession;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Metagame.MainPage
{
	public class RoomListElementPlayerInfoView : MonoBehaviour
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Text _nickNameText;
		[SerializeField]
		private SyncImage _avatarImage;
		[SerializeField]
		private string _emptyNickName;
		
		public void Enter(PlayerViewData data)
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
		
		private void _Enter(PlayerViewData data)
		{
			_nickNameText.text = data.NickName;
		}
		
		private void _EnterEmpty()
		{
			_nickNameText.text = _emptyNickName;
			_avatarImage.Clear();
		}

		public async UniTask LoadAsset(PlayerViewData viewData, IAssetSession assetSession, CancellationTokenSource token)
		{
			await _avatarImage.LoadSprite(assetSession.SyncLoad<Sprite>(AssetType.Avatar, viewData.AvatarImageKey));
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
