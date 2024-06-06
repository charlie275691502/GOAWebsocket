using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.GOA
{
	public class GOACharacterDetailViewData
	{
		
	}
	
	public class GOACharacterDetailView : MonoBehaviour
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Text _characterNameText;
		[SerializeField]
		private Text _skillNameText;
		[SerializeField]
		private Text _skillDescriptionText;

		public void RegisterCallback()
		{
			
		}

		public void Open(GOACharacterDetailViewData viewData)
		{
			_panel.SetActive(true);
		}

		public void Close()
		{
			_panel.SetActive(false);
		}
	}
}
