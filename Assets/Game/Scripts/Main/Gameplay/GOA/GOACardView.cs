using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Common.AssetSession;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.GOA
{
	public record CardViewDataState()
	{
		public record Empty() : CardViewDataState;
		public record Covered(bool IsChosen) : CardViewDataState;
		public record Open(bool IsPublicCard, bool IsChosen, string ImageKey) : CardViewDataState;
	}
	
	public record GOACardViewData(CardViewDataState State);
	
	public class GOACardView : MonoBehaviour
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

		public void Render(GOACardViewData viewData)
		{
			_panel.SetActive(viewData.State is not CardViewDataState.Empty);
			switch(viewData.State)
			{
				case CardViewDataState.Empty:
					_panel.SetActive(false);
					break;
				
				case CardViewDataState.Covered Info:
					_panel.SetActive(true);
					_image.LoadSprite(
						Info.IsChosen
							? AssetType.GOAPublicCardChosen
							: AssetType.GOAPublicCardNormal,
						GOACardUtility.COVERED_CARD_IMAGE_KEY);
					break;
					
				case CardViewDataState.Open Info:
					_panel.SetActive(true);
					_image.LoadSprite(
						Info.IsPublicCard
							? Info.IsChosen
								? AssetType.GOAPublicCardChosen
								: AssetType.GOAPublicCardNormal
							: AssetType.GOAStrategyCardNormal,
						Info.ImageKey);
					break;
			}
		}
	}
}
