using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Common.AssetSession;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.GOA
{
	public record PublicCardViewDataState()
	{
		public record Empty() : PublicCardViewDataState;
		public record Covered() : PublicCardViewDataState;
		public record Open(string ImageKey) : PublicCardViewDataState;
	}
	
	public record GOAPublicCardViewData(PublicCardViewDataState State, bool IsChosen);
	
	public class GOAPublicCardView : MonoBehaviour
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private AsyncImage _image;
		[SerializeField]
		private Button _button;

		public void RegisterCallback(IAssetSession assetSession, Action onClickButton)
		{
			_image.Initialize(assetSession);
			_button.onClick.AddListener(() => onClickButton?.Invoke());
		}

		public void Render(GOAPublicCardViewData viewData)
		{
			_panel.SetActive(viewData.State is not PublicCardViewDataState.Empty);
			switch(viewData.State)
			{
				case PublicCardViewDataState.Empty:
					_panel.SetActive(false);
					break;
				
				case PublicCardViewDataState.Covered:
					_panel.SetActive(true);
					_image.LoadSprite(
						viewData.IsChosen
							? AssetType.GOAPublicCardChosen
							: AssetType.GOAPublicCardNormal,
						GOACardUtility.COVERED_CARD_IMAGE_KEY);
					break;
					
				case PublicCardViewDataState.Open Info:
					_panel.SetActive(true);
					_image.LoadSprite(
						viewData.IsChosen
							? AssetType.GOAPublicCardChosen
							: AssetType.GOAPublicCardNormal,
						Info.ImageKey);
					break;
			}
			// viewDataOpt.Match(
			// 	viewData => 
			// 	{
			// 		_panel.SetActive(true);
			// 		_image.LoadSprite(AssetType.GOAStrategyCardDetail, viewData.ImageKey);
			// 	},
			// 	() => _panel.SetActive(false)
		}
	}
}
