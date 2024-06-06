using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.GOA
{
	public class GOAHandCardsViewData
	{
		
	}
	
	public class GOAHandCardsView : MonoBehaviour
	{
		[SerializeField]
		private Transform _handCardsFolder;

		public void RegisterCallback(Action onClickButton)
		{
		}

		public void Render(GOABoardViewData viewData)
		{
			
		}
	}
}
