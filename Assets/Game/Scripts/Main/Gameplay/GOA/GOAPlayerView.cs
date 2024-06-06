using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Common.AssetSession;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.GOA
{
	public record GOAPlayerViewData(
		bool IsSelf,
		bool IsTurn,
		string AvatarImageKey,
		bool IsBot,
		string NickName,
		string CharacterName,
		string SkillDescription,
		int PublicCardCount,
		int StrategyCardCount,
		int Power,
		int PowerLimit
	);
	
	public class GOAPlayerView : MonoBehaviour
	{
		[SerializeField]
		private GameObject _takingTurnIndicatorGameObject;
		[SerializeField]
		private GameObject _notTakingTurnIndicatorGameObject;
		[SerializeField]
		private AsyncImage _avatarImage;
		[SerializeField]
		private GameObject _botIndicatorGameObject;
		[SerializeField]
		private GameObject _playerIndicatorGameObject;
		[SerializeField]
		private Text _nickNameText;
		[SerializeField]
		private Text _characterNameText;
		[SerializeField]
		private Text _skillDescriptionText;
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
			_avatarImage.Initialize(assetSession);
		}

		public void Render(GOAPlayerViewData viewData)
		{
			_takingTurnIndicatorGameObject.SetActive(viewData.IsTurn);
			_notTakingTurnIndicatorGameObject.SetActive(!viewData.IsTurn);
			
			_avatarImage.LoadSprite(
				viewData.IsSelf 
					? AssetType.GOACharacterMid
					: AssetType.GOACharacterSmall,
				viewData.AvatarImageKey);
			
			_botIndicatorGameObject.SetActive(viewData.IsBot);
			_playerIndicatorGameObject.SetActive(!viewData.IsBot);
			
			_nickNameText.text = viewData.NickName;
			_characterNameText.text = viewData.CharacterName;
			_skillDescriptionText.text = viewData.SkillDescription;
			
			_publicCardCountText.text = viewData.PublicCardCount.ToString();
			_strategyCardCountText.text = viewData.StrategyCardCount.ToString();
			_powerText.text = viewData.Power.ToString();
			_powerLimitText.text = viewData.PowerLimit.ToString();
		}
	}
}
