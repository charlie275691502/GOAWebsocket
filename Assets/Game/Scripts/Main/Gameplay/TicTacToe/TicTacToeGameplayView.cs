using Common.LinqExtension;
using EnhancedUI.EnhancedScroller;
using Metagame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gameplay.TicTacToe
{
	public interface ITicTacToeGameplayView
	{
		void RegisterCallback(DiContainer container, Action<int> onClickClickPositionElement, Action onClickReturnHomeButton);
		void Render(TicTacToeGameplayProperty prop);
	}

	public class TicTacToeGameplayView : MonoBehaviour, ITicTacToeGameplayView
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
		private TicTacToePositionElementView[] _positionElements;
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

		private TicTacToeGameplayProperty _prop;

		void ITicTacToeGameplayView.RegisterCallback(DiContainer container, Action<int> onClickClickPositionElement, Action onClickReturnHomeButton)
		{
			_players.ForEach(player => player.Resolve(container));
			
			_summaryReturnHomeButton.onClick.AddListener(() => onClickReturnHomeButton?.Invoke());
			
			_positionElements
				.ForEach((view, index) => view.RegisterCallback(() => onClickClickPositionElement?.Invoke(index)));
		}

		void ITicTacToeGameplayView.Render(TicTacToeGameplayProperty prop)
		{
			if (_prop == prop)
				return;
			_prop = prop;
			
			switch (prop.State)
			{
				case TicTacToeGameplayState.Open:
					_Open();
					break;

				case TicTacToeGameplayState.Idle:
				case TicTacToeGameplayState.ClickPositionElement:
				case TicTacToeGameplayState.ClickReturnHome:
					_Render(prop);
					break;

				case TicTacToeGameplayState.Close:
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

		private void _Render(TicTacToeGameplayProperty prop)
		{
			_turnText.text = prop.Turn.ToString();
			_playerTurnIndicatorGameObject.SetActive(prop.IsPlayerTurn);
			_enemyTurnIndicatorGameObject.SetActive(!prop.IsPlayerTurn);
			_positionElements.ZipForEach(
				prop.PositionProperties,
				(view, property) => view.Render(property));
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
