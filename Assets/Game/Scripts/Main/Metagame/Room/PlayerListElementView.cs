using Common;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using Zenject;
using Cysharp.Threading.Tasks;
using System.Threading;
using Common.AssetSession;

namespace Metagame
{
	public class PlayerListElementView : SimpleEnhancedScrollerElement<PlayerViewData>
	{
		[SerializeField]
		private Text _nickNameText;
		[SerializeField]
		private SyncImage _avatarImage;
		[SerializeField]
		private string _emptyNickName;
		
		private IAssetSession _assetSession;
		
		
#region EnhancedScrollerElement
		
		protected override void _Update()
		{
			
		}
		
		protected override void _Resolve(DiContainer container)
		{
			_assetSession = container.Resolve<IAssetSession>();
		}
		
		protected override void _Display(PlayerViewData viewData)
		{
			_nickNameText.text = viewData.NickName;
		}
		
		protected override void _DisplayEmpty()
		{
			_nickNameText.text = _emptyNickName;
			_avatarImage.Clear();
		}
		
		protected override void _Refresh(PlayerViewData viewData)
		{
			
		}

		protected override async UniTask _LoadAsset(PlayerViewData viewData, CancellationTokenSource token)
		{
			await _avatarImage.LoadSprite(_assetSession.SyncLoad<Sprite>(AssetType.Avatar, viewData.AvatarImageKey));
		}

		protected override void _Leave()
		{
			_nickNameText.text = string.Empty;
		}
		
		protected override void _Register()
		{
			
		}
		
		protected override void _Unregister()
		{
			
		}
		
#endregion
	}
}