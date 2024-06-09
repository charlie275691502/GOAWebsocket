using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands.Merge;
using Common;
using Common.AssetSession;
using Data.Sheet;
using I2.Loc;
using JetBrains.Annotations;
using Optional;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.GOA
{
	public record GOAPlayerViewData(
		bool IsSelf,
		bool IsTurn,
		string CharacterImageKey,
		bool IsBot,
		string NickName,
		string CharacterNameKey,
		string SkillDescriptionKey,
		int PublicCardCount,
		int StrategyCardCount,
		int Power,
		int PowerLimit
	)
	{
		public GOAPlayerViewData(GOAPlayerData data, bool isSelf, int takingTurnPlayerId, IGoogleSheetLoader googleSheetLoader) : this(
			isSelf,
			takingTurnPlayerId == data.Player.Id,
			googleSheetLoader.Container.GOACharacters
				.GetRow(data.CharacterKey)
				.Map(character => character.ImageKey)
				.ValueOr(string.Empty),
			data.IsBot,
			data.Player.NickName,
			googleSheetLoader.Container.GOACharacters
				.GetRow(data.CharacterKey)
				.Map(character => character.NameKey)
				.ValueOr(string.Empty),
			googleSheetLoader.Container.GOACharacters
				.GetRow(data.CharacterKey)
				.Map(character => character.SkillDescriptionKey)
				.ValueOr(string.Empty),
			data.PublicCardCount,
			data.StrategyCardCount,
			data.Power,
			data.PowerLimit
		){}
	}
	
	public class GOAPlayerView : MonoBehaviour
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private GameObject _takingTurnIndicatorGameObject;
		[SerializeField]
		private GameObject _notTakingTurnIndicatorGameObject;
		[SerializeField]
		private AsyncImage _characterImage;
		[SerializeField]
		private GameObject _botIndicatorGameObject;
		[SerializeField]
		private GameObject _playerIndicatorGameObject;
		[SerializeField]
		private Text _nickNameText;
		[SerializeField]
		private Localize _characterNameLocalize;
		[SerializeField]
		private Localize _skillDescriptionLocalize;
		[SerializeField]
		private Text _publicCardCountText;
		[SerializeField]
		private Text _strategyCardCountText;
		[SerializeField]
		private Text _powerText;
		[SerializeField]
		private Text _powerLimitText;
		[SerializeField]
		private Transform _buffFolder;
		[SerializeField]
		private Button _button;

		public void RegisterCallback(IAssetSession assetSession, Action onClickButton)
		{
			_button.onClick.AddListener(() => onClickButton?.Invoke());
			_characterImage.Initialize(assetSession);
		}

		public void Render(Option<GOAPlayerViewData> viewDataOpt)
		{
			viewDataOpt.Match(
				viewData =>
				{
					_panel.SetActive(true);
					_takingTurnIndicatorGameObject.SetActive(viewData.IsTurn);
					_notTakingTurnIndicatorGameObject.SetActive(!viewData.IsTurn);
					
					_characterImage.LoadSprite(
						viewData.IsSelf 
							? AssetType.GOACharacterMid
							: AssetType.GOACharacterSmall,
						viewData.CharacterImageKey);
					
					_botIndicatorGameObject.SetActive(viewData.IsBot);
					_playerIndicatorGameObject.SetActive(!viewData.IsBot);
					
					_nickNameText.text = viewData.NickName;
					_characterNameLocalize.SetTerm(viewData.CharacterNameKey);
					_skillDescriptionLocalize.SetTerm(viewData.SkillDescriptionKey);
					
					_publicCardCountText.text = viewData.PublicCardCount.ToString();
					_strategyCardCountText.text = viewData.StrategyCardCount.ToString();
					_powerText.text = viewData.Power.ToString();
					_powerLimitText.text = viewData.PowerLimit.ToString();
				},
				() => _panel.SetActive(false)
			);
		}
	}
}
