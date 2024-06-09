using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.PlatformSupport.IL2CPP;
using I2.Loc;
using Optional;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.GOA
{
	public record GOACharacterDetailViewData(
		string CharacterNameKey,
		string SkillNameKey,
		string SkillDescriptionKey
	);
	
	public class GOACharacterDetailView : MonoBehaviour
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Localize _characterNameLocalize;
		[SerializeField]
		private Localize _skillNameLocalize;
		[SerializeField]
		private Localize _skillDescriptionLocalize;

		public void RegisterCallback()
		{
			
		}

		public void Render(Option<GOACharacterDetailViewData> viewDataOpt)
		{
			viewDataOpt.Match(
				viewData => 
				{
					_panel.SetActive(true);
					_characterNameLocalize.SetTerm(viewData.CharacterNameKey);
					_skillNameLocalize.SetTerm(viewData.SkillNameKey);
					_skillDescriptionLocalize.SetTerm(viewData.SkillDescriptionKey);
				},
				() => _panel.SetActive(false)
			);
		}
	}
}
