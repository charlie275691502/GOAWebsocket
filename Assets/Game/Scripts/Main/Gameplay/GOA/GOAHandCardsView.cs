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
	public record GOAHandCardsViewData(
		GOACardViewData[] Cards
	);

	public class GOAHandCardsView : MonoBehaviour
	{
		[SerializeField]
		private Transform _cardsFolder;
		[SerializeField]
		private GameObjectPool _cardPool;

		private IAssetSession _assetSession;
		private Action<int> _onClickCard;
		private List<GOACardView> _cards = new List<GOACardView>();

		public void RegisterCallback(IAssetSession assetSession, Action<int> onClickCard)
		{
			_assetSession = assetSession;
			_onClickCard = onClickCard;
		}

		public void Render(GOAHandCardsViewData viewData)
		{
			_InstantiateCards(viewData.Cards.Count());

			_cards.ZipForEach(
				viewData.Cards,
				(card, viewData, index) =>
				{
					card.RegisterCallback(_assetSession, () => _onClickCard?.Invoke(index));
					card.Render(viewData);
				}
			);
		}

		private void _InstantiateCards(int count)
		{
			for (int i = _cards.Count() - 1; i >= count; i--)
			{
				_cardPool.ReturnGameObject(_cards[i].gameObject);
				_cards.RemoveAt(i);
			}

			for (int i = _cards.Count(); i < count; i++)
			{
				var card = _cardPool.GetGameObject();
				card.transform.parent = _cardsFolder;
				var cardView = card.GetComponent<GOACardView>();
				_cards.Add(cardView);
			}
		}
	}
}
