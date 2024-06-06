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
	public record GOAStrategyCardDetaialViewData(
		string ImageKey
	);
	
	public class GOAStrategyCardDetaialView : MonoBehaviour
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private AsyncImage _image;
		[SerializeField]
		private Button _useButton;
		
		private IAssetSession _assetSession;

		public void RegisterCallback(IAssetSession assetSession, Action onClickUseButton)
		{
			_assetSession = assetSession;
			_useButton.onClick.AddListener(() => onClickUseButton?.Invoke());
		}

		public void Render(Option<GOAStrategyCardDetaialViewData> viewDataOpt)
		{
			viewDataOpt.Match(
				viewData => 
				{
					_panel.SetActive(true);
					_image.LoadSprite(AssetType.GOAStrategyCardDetail, viewData.ImageKey);
				},
				() => _panel.SetActive(false)
			);
		}
	}
}
