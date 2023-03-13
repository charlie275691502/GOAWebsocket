using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

namespace Metagame
{
	public class MessageListElementView : EnhancedScrollerCellView, ISimpleEnhancedScrollerElement<MessageViewData>
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Image _avatar;
		[SerializeField]
		private Text _contentText;
		[SerializeField]
		private Text _nickNameText;
		
		public void Enter(MessageViewData viewData)
		{
			_Enter(viewData);
			_Register();
			_panel.SetActive(true);
		}
		
		public void Leave()
		{
			_Leave();
			_Unregister();
			_panel.SetActive(false);
		}
		
		private void _Enter(MessageViewData viewData)
		{
			_contentText.text = viewData.Content;
			_nickNameText.text = viewData.NickName;
		}
		
		private void _Leave()
		{
			_contentText.text = string.Empty;
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