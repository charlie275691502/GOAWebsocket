using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.GOA
{
	public record GOABoardViewData();
	
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
		private Transform _publicCardsFolder;

		public void RegisterCallback(Action onClickButton)
		{
		}

		public void Render(GOABoardViewData viewData)
		{
			
		}
	}
}
