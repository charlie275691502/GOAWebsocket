using Common.AssetSession;
using Common.LinqExtension;
using EnhancedUI.EnhancedScroller;
using Metagame;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gameplay.GOA
{
	public interface IGOAGameplayView
	{
		void RegisterCallback(
			IAssetSession assetSession,
			Action<int> onClickBoardCard,
			Action<int> onClickHandCard,
			Action onClickUseCardButton,
			Action onClickReleaseCardButton,
			Action onClickReleaseRequirementConfirmButton,
			Action onClickStrategyRequirementConfirmButton,
			Action onClickEndTurn,
			Action onClickEndCongress);
		void Render(GOAGameplayProperty prop);
	}

	public class GOAGameplayView : MonoBehaviour, IGOAGameplayView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private GOAPlayerView _selfPlayer;
		[SerializeField]
		private List<GOAPlayerView> _enemyPlayers;
		[SerializeField]
		private GOABoardView _board;
		[SerializeField]
		private GOAHandCardsView _handCards;
		[SerializeField]
		private GOACharacterDetailView _characterDetail;
		[SerializeField]
		private GOAPublicCardDetaialView _publicCardDetail;
		[SerializeField]
		private GOAStrategyCardDetaialView _strategyCardDetail;
		[SerializeField]
		private Text _turnText;
		[SerializeField]
		private Button _releaseButton;
		[SerializeField]
		private GameObject _releaseButtonGameObject;
		[SerializeField]
		private Button _releaseRequirementConfirmButton;
		[SerializeField]
		private GameObject _releaseRequirementConfirmButtonGameObject;
		[SerializeField]
		private Button _strategyRequirementConfirmButton;
		[SerializeField]
		private GameObject _strategyRequirementConfirmButtonGameObject;
		[SerializeField]
		private Button _endTurnButton;
		[SerializeField]
		private GameObject _endTurnButtonGameObject;
		[SerializeField]
		private Button _endCongressButton;
		[SerializeField]
		private GameObject _endCongressButtonGameObject;
		[SerializeField]
		private GameObject _chooseBoardCardPhaseHintGameObject;
		[SerializeField]
		private GameObject _actionPhaseHintGameObject;
		[SerializeField]
		private GameObject _congressPhaseHintGameObject;
		[SerializeField]
		private GameObject _useReformHintGameObject;
		[SerializeField]
		private GameObject _useExpandHintGameObject;
		[SerializeField]
		private GameObject _choosingReleaseRequirementHintGameObject;
		[SerializeField]
		private GameObject _choosingStrategyRequirementHintGameObject;

		private GOAGameplayProperty _prop;

		void IGOAGameplayView.RegisterCallback(
			IAssetSession assetSession,
			Action<int> onClickBoardCard,
			Action<int> onClickHandCard,
			Action onClickUseCardButton,
			Action onClickReleaseCardButton,
			Action onClickReleaseRequirementConfirmButton,
			Action onClickStrategyRequirementConfirmButton,
			Action onClickEndTurn,
			Action onClickEndCongress)
		{
			_selfPlayer.RegisterCallback(
				assetSession,
				() => { });
			_enemyPlayers
				.ForEach(enemyPlayer => enemyPlayer.RegisterCallback(
					assetSession,
					() => { }));
			_board.RegisterCallback(
				assetSession,
				onClickBoardCard);
			_handCards.RegisterCallback(
				assetSession,
				onClickHandCard);
			_characterDetail.RegisterCallback();
			_publicCardDetail.RegisterCallback(
				assetSession,
				onClickUseCardButton);
			_strategyCardDetail.RegisterCallback(
				assetSession,
				onClickUseCardButton);

			_releaseButton.onClick.AddListener(() => onClickReleaseCardButton?.Invoke());
			_releaseRequirementConfirmButton.onClick.AddListener(() => onClickReleaseRequirementConfirmButton?.Invoke());
			_strategyRequirementConfirmButton.onClick.AddListener(() => onClickStrategyRequirementConfirmButton?.Invoke());
			_endTurnButton.onClick.AddListener(() => onClickEndTurn?.Invoke());
			_endCongressButton.onClick.AddListener(() => onClickEndCongress?.Invoke());
		}

		void IGOAGameplayView.Render(GOAGameplayProperty prop)
		{
			if (_prop == prop)
				return;
			_prop = prop;

			switch (prop.State)
			{
				case GOAGameplayState.Open:
					_Open();
					break;

				case GOAGameplayState.Idle:
				case GOAGameplayState.ClickBoardCard:
				case GOAGameplayState.ClickHandCard:
				case GOAGameplayState.ClickUseButton:
				case GOAGameplayState.ClickReleaseButton:
				case GOAGameplayState.ClickReleaseRequirementConfirm:
				case GOAGameplayState.ClickStrategyRequirementConfirm:
				case GOAGameplayState.ClickEndTurn:
				case GOAGameplayState.ClickEndCongress:
					_Render(prop);
					break;

				case GOAGameplayState.Close:
					_Close();
					break;

				default:
					break;
			}
		}

		private void _Open()
		{
			_panel.SetActive(true);
		}

		private void _Close()
		{
			_panel.SetActive(false);
		}

		private void _Render(GOAGameplayProperty prop)
		{
			_selfPlayer.Render(prop.SelfPlayer.Some());
			_enemyPlayers.ZipForEach(
				prop.EnemyPlayers.ExtendUntil(_enemyPlayers.Count()),
				(enemyPlayer, viewDataOpt) => enemyPlayer.Render(viewDataOpt));
			_board.Render(prop.Board);
			_handCards.Render(prop.HandCards);
			_characterDetail.Render(prop.CharacterDetailOpt);
			_publicCardDetail.Render(prop.PublicCardDetailOpt);
			_strategyCardDetail.Render(prop.StrategyCardDetailOpt);

			_turnText.text = prop.Turn.ToString();
			_releaseRequirementConfirmButtonGameObject.SetActive(prop.IsReleaseRequirementActionPhase);
			_strategyRequirementConfirmButtonGameObject.SetActive(prop.IsStrategyRequirementActionPhase);
			_releaseButtonGameObject.SetActive(prop.ShowReleaseButton);
			_endTurnButtonGameObject.SetActive(prop.ShowEndTurnButton);
			_endCongressButtonGameObject.SetActive(prop.ShowEndCongressButton);

			_choosingReleaseRequirementHintGameObject.SetActive(prop.IsReleaseRequirementActionPhase);
			_choosingStrategyRequirementHintGameObject.SetActive(prop.IsStrategyRequirementActionPhase);
			_chooseBoardCardPhaseHintGameObject.SetActive(prop.ShowChooseBoardCardPhaseHint);
			_actionPhaseHintGameObject.SetActive(prop.ShowActionPhaseHint);
			_congressPhaseHintGameObject.SetActive(prop.ShowCongressPhaseHint);
			_useReformHintGameObject.SetActive(prop.ShowUseReformHint);
			_useExpandHintGameObject.SetActive(prop.ShowUseExpandHint);
		}
	}
}
