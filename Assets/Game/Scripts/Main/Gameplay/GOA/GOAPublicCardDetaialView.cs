using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Common.AssetSession;
using Optional;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.GOA
{
	public record GOAPublicCardDetaialViewData(
		string ImageKey
	);
	
	public class GOAPublicCardDetaialView : MonoBehaviour
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private AsyncImage _image;
		[SerializeField]
		private Button _useButton;
		
		public void RegisterCallback(IAssetSession assetSession, Action onClickUseButton)
		{
			_image.Initialize(assetSession);
			_useButton.onClick.AddListener(() => onClickUseButton?.Invoke());
		}

		public void Render(Option<GOAPublicCardDetaialViewData> viewDataOpt)
		{
			viewDataOpt.Match(
				viewData => 
				{
					_panel.SetActive(true);
					_image.LoadSprite(AssetType.GOAPublicCardDetail, viewData.ImageKey);
				},
				() => _panel.SetActive(false)
			);
		}
	}
}
