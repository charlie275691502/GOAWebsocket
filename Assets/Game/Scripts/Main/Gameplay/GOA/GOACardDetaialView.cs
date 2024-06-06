using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Optional;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.GOA
{
	public record GOACardDetaialViewData();
	
	public class GOACardDetaialView : MonoBehaviour
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private AsyncImage _image;
		[SerializeField]
		private Button _useButton;

		public void RegisterCallback(Action onClickUseButton)
		{
			_useButton.onClick.AddListener(() => onClickUseButton?.Invoke());
		}

		public void Render(Option<GOACardDetaialViewData> viewDataOpt)
		{
			viewDataOpt.Match(
				viewData => 
				{
					_panel.SetActive(true);
				},
				() => _panel.SetActive(false)
			);
		}
	}
}
