using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.PlatformSupport.IL2CPP;
using Optional;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.GOA
{
	public record GOACharacterDetailViewData();
	
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

		public void Render(Option<GOACharacterDetailViewData> viewDataOpt)
		{
			viewDataOpt.Match(
				viewData => 
				{
					_panel.SetActive(true);
				},
				() => _panel.SetActive(false)
			);
		}

		public void Close()
		{
		}
	}
}
