using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.AssetSession;
using Common.LinqExtension;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.GOA
{
	public record GOABoardViewData(
		int DrawCardCount,
		int GraveCardCount,
		GOACardViewData[] Cards
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
		private GameObjectPool _cardPool;
		[SerializeField]
		private Transform _cardsFolder;
		
		private IAssetSession _assetSession;
		private Action<int> _onClickCard;
		private List<GOACardView> _cards = new List<GOACardView>();

		public void RegisterCallback(IAssetSession assetSession, Action<int> onClickCard)
		{
			_assetSession = assetSession;
			_onClickCard = onClickCard;
		}

		public void Render(GOABoardViewData viewData)
		{
			if(viewData.Cards.Count() != _cards.Count())
			{
				_InstantiateCards(viewData.Cards.Count());
			}
			
			_cards.ZipForEach(
				viewData.Cards,
				(publicCard, viewData) => publicCard.Render(viewData)
			);
			
			_drawPileMultipleGameObject.SetActive(viewData.DrawCardCount > 1);
			_drawPileOneGameObject.SetActive(viewData.DrawCardCount == 1);
			_drawPileEmptyGameObject.SetActive(viewData.DrawCardCount == 0);
			_gravePileMultipleGameObject.SetActive(viewData.GraveCardCount > 1);
			_gravePileOneGameObject.SetActive(viewData.GraveCardCount == 1);
			_gravePileEmptyGameObject.SetActive(viewData.GraveCardCount == 0);
		}
		
		private void _InstantiateCards(int count)
		{
			_cards.ForEach(publicCard =>_cardPool.ReturnGameObject(publicCard.gameObject));
			for(int i=0; i<count; i++)
			{
				var index = i;
				var publicCard = _cardPool.GetGameObject();
				publicCard.transform.parent = _cardsFolder;
				var publicCardView = publicCard.GetComponent<GOACardView>();
				publicCardView.RegisterCallback(_assetSession, () => _onClickCard?.Invoke(index));
				_cards.Add(publicCardView);
			}
		}
	}
}
