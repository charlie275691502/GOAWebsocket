using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.GOA
{
	public record GOAPlayerViewData();
	
	public class GOAPlayerView : MonoBehaviour
	{
		[SerializeField]
		private GameObject _takingTurnIndicatorGameObject;
		[SerializeField]
		private GameObject _notTakingTurnIndicatorGameObject;
		[SerializeField]
		private SyncImage _avatar;
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

		public void RegisterCallback(Action onClickButton)
		{
			_button.onClick.AddListener(() => onClickButton?.Invoke());
		}

		public void Render(GOAPlayerViewData viewData)
		{
			
		}
	}
}
