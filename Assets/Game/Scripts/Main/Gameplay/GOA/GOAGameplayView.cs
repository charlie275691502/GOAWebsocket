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
		void RegisterCallback(DiContainer container, Action<int> onClickClickPositionElement, Action onClickReturnHomeButton);
		void Render(GOAGameplayProperty prop);
	}

	public class GOAGameplayView : MonoBehaviour, IGOAGameplayView
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Text _turnText;
		[SerializeField]
		private GameObject _playerTurnIndicatorGameObject;
		[SerializeField]
		private GameObject _enemyTurnIndicatorGameObject;
		[SerializeField]
		private List<PlayerListElementView> _players;
		[SerializeField]
		private GameObject _summaryPanel;
		[SerializeField]
		private Text _winnerNameText;
		[SerializeField]
		private Text _summaryTurnsText;
		[SerializeField]
		private Button _summaryReturnHomeButton;

		private GOAGameplayProperty _prop;

		void IGOAGameplayView.RegisterCallback(DiContainer container, Action<int> onClickClickPositionElement, Action onClickReturnHomeButton)
		{
			_players.ForEach(player => player.Resolve(container));
			
			_summaryReturnHomeButton.onClick.AddListener(() => onClickReturnHomeButton?.Invoke());
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
				case GOAGameplayState.ClickPositionElement:
				case GOAGameplayState.ClickReturnHome:
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
			_turnText.text = prop.Turn.ToString();
			_playerTurnIndicatorGameObject.SetActive(prop.IsPlayerTurn);
			_enemyTurnIndicatorGameObject.SetActive(!prop.IsPlayerTurn);
			_players.ZipForEach(
				prop.PlayerViewDatas,
				(view, viewData) => view.Display(new EnhancedScrollerElementViewData<PlayerViewData>(viewData)));
			
			_summaryPanel.SetActive(prop.IsGameEnd);
			if (prop.IsGameEnd)
			{
				_winnerNameText.text = prop.WinnerName;
				_summaryTurnsText.text = prop.SummaryTurns.ToString();
			}
		}
	}
}
