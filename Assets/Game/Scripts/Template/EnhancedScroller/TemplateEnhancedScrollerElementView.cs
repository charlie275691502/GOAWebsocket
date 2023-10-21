using System;
using System.Collections;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Template
{
	public class TemplateViewData
	{
		
	}
	
	public class TemplateEnhancedScrollerElementView : SimpleEnhancedScrollerElement<TemplateViewData>
	{
		[SerializeField]
		private Button _button;
		
		public Action OnClick;
		
#region EnhancedScrollerElement
		
		protected override void _Update()
		{
			
		}
		
		protected override void _Resolve(DiContainer container)
		{
			
		}
		
		protected override void _Display(TemplateViewData viewData)
		{
			
		}
		
		protected override void _DisplayEmpty()
		{
			
		}
		
		protected override void _Refresh(TemplateViewData viewData)
		{
			
		}
		
		protected override IEnumerator _LoadAsset(TemplateViewData viewData)
		{
			yield break;
		}
		
		protected override void _Leave()
		{
			
		}
		
		protected override void _Register()
		{
			_button.onClick.AddListener(_OnClick);
		}
		
		protected override void _Unregister()
		{
			OnClick = null;
			_button.onClick.RemoveAllListeners();
		}
		
#endregion

		private void _OnClick()
		{
			OnClick?.Invoke();
		}
	}
}