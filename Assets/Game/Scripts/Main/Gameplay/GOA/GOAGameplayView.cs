using Common.LinqExtension;
using EnhancedUI.EnhancedScroller;
using Metagame;
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
		void RegisterCallback();
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
		private Button _button;

		private GOAGameplayProperty _prop;

		void IGOAGameplayView.RegisterCallback()
		{
			// _button.onClick.AddListener(() => onClickReturnHomeButton?.Invoke());
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
			_selfPlayer.Render(prop.SelfPlayer);
			_enemyPlayers.ZipForEach(
				prop.EnemyPlayers,
				(enemyPlayer, viewData) => enemyPlayer.Render(viewData));
			_board.Render(prop.Board);
			_handCards.Render(prop.HandPublicCards);
			_characterDetail.Render(prop.CharacterDetailOpt);
			_publicCardDetail.Render(prop.PublicCardDetailOpt);
			_strategyCardDetail.Render(prop.StrategyCardDetailOpt);
		}
	}
}
