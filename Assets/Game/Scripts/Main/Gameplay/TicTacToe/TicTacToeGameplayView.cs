using Common.LinqExtension;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.TicTacToe
{
	public interface ITicTacToeGameplayView
	{
		void RegisterCallback(Action<int> onClickClickPositionElement, Action onClickConfirmPositionButton);
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
		private GameObject _confirmPositionButtonGameObject;
		[SerializeField]
		private Button _confirmPositionButton;
		[SerializeField]
		private Button _resignButton;

		private TicTacToeGameplayProperty _prop;

		void ITicTacToeGameplayView.RegisterCallback(Action<int> onClickClickPositionElement, Action onClickConfirmPositionButton)
		{
			_confirmPositionButton.onClick.AddListener(() => onClickConfirmPositionButton?.Invoke());
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
			_confirmPositionButtonGameObject.SetActive(prop.ShowConfirmPositionButton);
		}
	}
}
