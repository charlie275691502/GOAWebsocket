using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.PlatformSupport.IL2CPP;
using Optional;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.GOA
{
	public record GOACharacterDetailViewData(
		string CharacterName,
		string SkillName,
		string SkillDescription
	);
	
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
					_characterNameText.text = viewData.CharacterName;
					_skillNameText.text = viewData.SkillName;
					_skillDescriptionText.text = viewData.SkillDescription;
				},
				() => _panel.SetActive(false)
			);
		}
	}
}
