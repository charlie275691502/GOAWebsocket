using System.Collections;
using Common;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using Zenject;

namespace Metagame
{
	public class PlayerListElementView : SimpleEnhancedScrollerElement<PlayerData>
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Text _nickNameText;
		[SerializeField]
		private Image _avatarImage;
		[SerializeField]
		private string _emptyNickName;
		
		
#region EnhancedScrollerElement
		
		protected override void _Update()
		{
			
		}
		
		protected override void _Resolve(DiContainer container)
		{
			
		}
		
		protected override void _Display(PlayerData viewData)
		{
			_nickNameText.text = viewData.NickName;
		}
		
		protected override void _DisplayEmpty()
		{
			_nickNameText.text = _emptyNickName;
		}
		
		protected override void _Refresh(PlayerData viewData)
		{
			
		}
		
		protected override IEnumerator _LoadAsset(PlayerData viewData)
		{
			yield break;
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