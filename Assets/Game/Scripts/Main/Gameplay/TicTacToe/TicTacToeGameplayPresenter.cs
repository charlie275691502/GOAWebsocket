using System;
using System.Linq;
using Common.Warning;
using Cysharp.Threading.Tasks;
using Web;

namespace Gameplay.TicTacToe
{
	public abstract record TicTacToeGameplayState
	{
		public record Open() : TicTacToeGameplayState;
		public record Idle() : TicTacToeGameplayState;
		public record Close() : TicTacToeGameplayState;
	}

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

		private TicTacToeGameplayProperty _prop;

		public const int BOARD_SIZE = 3;

		public TicTacToeGameplayPresenter(IHTTPPresenter hTTPPresenter, IWarningPresenter warningPresenter, ITicTacToeGameplayView view)
		{
			_hTTPPresenter = hTTPPresenter;
			_warningPresenter = warningPresenter;
			_view = view;

			_view.RegisterCallback();
		}

		async UniTask ITicTacToeGameplayPresenter.Run(TicTacToeGameData gameData)
		{
			_prop = new TicTacToeGameplayProperty(
				new TicTacToeGameplayState.Open(),
				0,
				false,
				Enumerable.Repeat(
					new TicTacToePositionElementView.Property(new TicTacToePositionElementView.State.Empty()), BOARD_SIZE * BOARD_SIZE).ToArray());

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
	}
}