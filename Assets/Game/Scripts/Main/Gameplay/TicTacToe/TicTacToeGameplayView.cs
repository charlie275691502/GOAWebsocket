using Common.LinqExtension;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.TicTacToe
{
	public interface ITicTacToeGameplayView
	{
		void RegisterCallback();
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
		private Button _confirmPositionButton;
		[SerializeField]
		private Button _resignButton;

		private Action _onClickConfirmPositionButton;
		private Action _onClickResignButton;

		private TicTacToeGameplayProperty _prop;

		void ITicTacToeGameplayView.RegisterCallback()
		{
			//_onConfirm = onConfirm;

			//_button.onClick.AddListener(_OnConfirm);
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
		}

		private void _OnConfirm()
		{
			//_onConfirm?.Invoke();
		}
	}
}
