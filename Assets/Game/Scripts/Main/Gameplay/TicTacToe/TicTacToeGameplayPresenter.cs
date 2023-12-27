using System;
using System.Collections.Generic;
using System.Linq;
using Common.Warning;
using Cysharp.Threading.Tasks;
using Optional;
using Optional.Collections;
using Web;

namespace Gameplay.TicTacToe
{
	public abstract record TicTacToeGameplayState
	{
		public record Open() : TicTacToeGameplayState;
		public record Idle() : TicTacToeGameplayState;
		public record Close() : TicTacToeGameplayState;
	}

	public record TicTacToeGameplayModel(
		int SelfPlayerId,
		int SelfPlayerTeam,
		int[] Positions,
		Option<int> GhostPosition);

	public record TicTacToeGameplayProperty(
		TicTacToeGameplayState State,
		int Turn,
		bool IsPlayerTurn,
		TicTacToePositionElementView.Property[] PositionProperties);

	public interface ITicTacToeGameplayPresenter
	{
		UniTask Run(TicTacToeGameData gameData);
	}

	public class TicTacToeGameplayPresenter : ITicTacToeGameplayPresenter
	{
		private IHTTPPresenter _hTTPPresenter;
		private IWarningPresenter _warningPresenter;
		private ITicTacToeGameplayView _view;

		private TicTacToeGameplayModel _model;
		private TicTacToeGameplayProperty _prop;

		public const int BOARD_SIZE = 3;

		public TicTacToeGameplayPresenter(
			IHTTPPresenter hTTPPresenter,
			IWarningPresenter warningPresenter,
			ITicTacToeGameplayView view)
		{
			_hTTPPresenter = hTTPPresenter;
			_warningPresenter = warningPresenter;
			_view = view;

			_view.RegisterCallback();
		}

		async UniTask ITicTacToeGameplayPresenter.Run(TicTacToeGameData gameData)
		{
			_model = new TicTacToeGameplayModel(
				gameData.SelfPlayerId,
				gameData.SelfPlayerTeam,
				gameData.Board.Positions,
				Option.None<int>());

			_prop = new TicTacToeGameplayProperty(
				new TicTacToeGameplayState.Open(),
				gameData.Board.Turn,
				gameData.SelfPlayerTeam == gameData.Board.TurnOfTeam,
				_GetPositionsProperty(_model.Positions, _model.GhostPosition));

			while (_prop.State is not TicTacToeGameplayState.Close)
			{
				_view.Render(_prop);
				switch (_prop.State)
				{
					case TicTacToeGameplayState.Open:
						_prop = _prop with { State = new TicTacToeGameplayState.Idle() };
						break;

					case TicTacToeGameplayState.Idle:
						break;

					case TicTacToeGameplayState.Close:
						break;

					default:
						break;
				}
				await UniTask.Yield();
			}

			_view.Render(_prop);
		}

		private void _ChangeStateIfIdle(TicTacToeGameplayState targetState, Action onChangeStateSuccess = null)
		{
			if (_prop.State is not TicTacToeGameplayState.Idle)
				return;

			onChangeStateSuccess?.Invoke();
			_prop = _prop with { State = targetState };
		}

		private TicTacToePositionElementView.Property[] _GetPositionsProperty(int[] Positions, Option<int> GhostPosition)
			=>
				Positions
					.Select(position => new TicTacToePositionElementView.Property(
						position switch
						{
							0 => new TicTacToePositionElementView.State.Empty(),
							1 => new TicTacToePositionElementView.State.Circle(GhostPosition.Contains(position)),
							2 => new TicTacToePositionElementView.State.Cross(GhostPosition.Contains(position)),
							_ => throw new System.NotImplementedException(),
						}))
					.ToArray();
	}
}