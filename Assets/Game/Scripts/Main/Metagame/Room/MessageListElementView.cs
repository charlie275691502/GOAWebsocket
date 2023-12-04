using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using Zenject;
using Cysharp.Threading.Tasks;
using System.Threading;
using Metagame.Room;
using Common;
using Common.AssetSession;

namespace Metagame
{
	public class MessageListElementView : SimpleEnhancedScrollerElement<MessageViewData>
	{
		[SerializeField]
		private Image _avatar;
		[SerializeField]
		private Text _contentText;
		[SerializeField]
		private Text _nickNameText;
		[SerializeField]
		private SyncImage _avatarImage;

		private IAssetSession _assetSession;

		#region EnhancedScrollerElement

		protected override void _Update()
		{
			
		}
		
		protected override void _Resolve(DiContainer container)
		{
			_assetSession = container.Resolve<IAssetSession>();
		}
		
		protected override void _Display(MessageViewData viewData)
		{
			_contentText.text = viewData.Content;
			_nickNameText.text = viewData.NickName;
		}
		
		protected override void _DisplayEmpty()
		{
			_contentText.text = string.Empty;
			_nickNameText.text = string.Empty;
			_avatarImage.Clear();
		}
		
		protected override void _Refresh(MessageViewData viewData)
		{

		}

		protected override async UniTask _LoadAsset(MessageViewData viewData, CancellationTokenSource token)
		{
			await _avatarImage.LoadSprite(_assetSession.SyncLoad<Sprite>(AssetType.Avatar, viewData.AvatarImageKey));
		}

		protected override void _Leave()
		{
			
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