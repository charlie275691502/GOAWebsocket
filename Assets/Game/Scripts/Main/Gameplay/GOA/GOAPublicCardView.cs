using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.GOA
{
	public class GOAPublicCardViewData
	{
		
	}
	
	public class GOAPublicCardView : MonoBehaviour
	{
		[SerializeField]
		private SyncImage _image;
		[SerializeField]
		private Button _button;

		public void RegisterCallback(Action onClickButton)
		{
			_button.onClick.AddListener(() => onClickButton?.Invoke());
		}

		public void Render(GOAPublicCardViewData viewData)
		{
			
		}
	}
}
