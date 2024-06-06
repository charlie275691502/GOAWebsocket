using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.AssetSession;
using Common.LinqExtension;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.GOA
{
	public record GOABoardViewData(
		int DrawCardCount,
		int GraveCardCount,
		GOAPublicCardViewData[] PublicCards
	);
	
	public class GOABoardView : MonoBehaviour
	{
		[SerializeField]
		private GameObject _drawPileMultipleGameObject;
		[SerializeField]
		private GameObject _drawPileOneGameObject;
		[SerializeField]
		private GameObject _drawPileEmptyGameObject;
		[SerializeField]
		private GameObject _gravePileMultipleGameObject;
		[SerializeField]
		private GameObject _gravePileOneGameObject;
		[SerializeField]
		private GameObject _gravePileEmptyGameObject;
		[SerializeField]
		private GOAPublicCardView _publicCardPrefab;
		[SerializeField]
		private Transform _publicCardsFolder;
		
		private IAssetSession _assetSession;
		private Action<int> _onClickCard;
		private List<GOAPublicCardView> _publicCards = new List<GOAPublicCardView>();

		public void RegisterCallback(IAssetSession assetSession, Action<int> onClickCard)
		{
			_assetSession = assetSession;
			_onClickCard = onClickCard;
		}

		public void Render(GOABoardViewData viewData)
		{
			if(viewData.PublicCards.Count() != _publicCards.Count())
			{
				_InstantiatePublicCards(viewData.PublicCards.Count());
			}
			
			_publicCards.ZipForEach(
				viewData.PublicCards,
				(publicCard, viewData) => publicCard.Render(viewData)
			);
			
			_drawPileMultipleGameObject.SetActive(viewData.DrawCardCount > 1);
			_drawPileOneGameObject.SetActive(viewData.DrawCardCount == 1);
			_drawPileEmptyGameObject.SetActive(viewData.DrawCardCount == 0);
			_gravePileMultipleGameObject.SetActive(viewData.GraveCardCount > 1);
			_gravePileOneGameObject.SetActive(viewData.GraveCardCount == 1);
			_gravePileEmptyGameObject.SetActive(viewData.GraveCardCount == 0);
		}
		
		private void _InstantiatePublicCards(int count)
		{
			_publicCards.ForEach(publicCard => Destroy(publicCard.gameObject));
			for(int i=0; i<count; i++)
			{
				var index = i;
				var publicCard = Instantiate(_publicCardPrefab, _publicCardsFolder);
				publicCard.RegisterCallback(_assetSession, () => _onClickCard?.Invoke(index));
				_publicCards.Add(publicCard);
			}
		}
	}
}
