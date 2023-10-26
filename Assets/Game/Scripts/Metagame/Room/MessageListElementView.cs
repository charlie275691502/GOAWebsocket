using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using Zenject;
using Cysharp.Threading.Tasks;
using System.Threading;

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
		
#region EnhancedScrollerElement
		
		protected override void _Update()
		{
			
		}
		
		protected override void _Resolve(DiContainer container)
		{
			
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
		}
		
		protected override void _Refresh(MessageViewData viewData)
		{

		}

		protected override async UniTask _LoadAsset(MessageViewData viewData, CancellationTokenSource token)
		{
			await UniTask.Yield();
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