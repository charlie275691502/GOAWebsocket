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
		string ImageKey,
		bool ActivateUseCardButton
	);
	
	public class GOAStrategyCardDetaialView : MonoBehaviour
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private AsyncImage _image;
		[SerializeField]
		private Button _useCardButton;
		
		public void RegisterCallback(IAssetSession assetSession, Action onClickUseCardButton)
		{
			_image.Initialize(assetSession);
			_useCardButton.onClick.AddListener(() => onClickUseCardButton?.Invoke());
		}

		public void Render(Option<GOAStrategyCardDetaialViewData> viewDataOpt)
		{
			viewDataOpt.Match(
				viewData => 
				{
					_panel.SetActive(true);
					_useCardButton.interactable = viewData.ActivateUseCardButton;
					_image.LoadSprite(AssetType.GOAStrategyCardDetail, viewData.ImageKey);
				},
				() => _panel.SetActive(false)
			);
		}
	}
}
