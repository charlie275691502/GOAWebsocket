using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.GOA
{
	public record GOAHandCardsViewData(
		GOAPublicCardViewData[] PublicCards
	);
	
	public class GOAHandCardsView : MonoBehaviour
	{
		[SerializeField]
		private Transform _handCardsFolder;

		public void RegisterCallback()
		{
		}

		public void Render(GOAHandCardsViewData viewData)
		{
			
		}
	}
}
