using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common;
using Common.LinqExtension;
using Common.Observable;
using Common.UniTaskExtension;
using Common.Warning;
using Cysharp.Threading.Tasks;
using Optional;
using Optional.Collections;
using Optional.Unsafe;
using Web;

namespace Gameplay.TicTacToe
{
	public abstract record TicTacToeGameplayState
	{
		public record Open() : TicTacToeGameplayState;
		public record Idle() : TicTacToeGameplayState;
		public record ClickPositionElement(int Position) : TicTacToeGameplayState;
		public record ClickPositionConfirmButton() : TicTacToeGameplayState;
		public record Close() : TicTacToeGameplayState;
	}

	public record TicTacToeGameplayModel(
		int SelfPlayerId,
		int SelfPlayerTeam,
		bool IsPlayerTurn)
	{
		public TicTacToeGameplayModel(
			Observable<int[]> positionsObs,
			Observable<Option<int>> ghostPositionObs) : this(
				0,
				0,
				false)
		{
			_ghostPositionObs = ghostPositionObs;
			_positionsObs = positionsObs;
		}

		private Observable<int[]> _positionsObs;
		public int[] Positions
		{
			get => _positionsObs.Value;
			set => _positionsObs.Value = value;
		}

		private Observable<Option<int>> _ghostPositionObs;
		public Option<int> GhostPositionOpt
		{
			get => _ghostPositionObs.Value;
			set => _ghostPositionObs.Value = value;
		}
	}

	public record TicTacToeGameplayProperty(
		TicTacToeGameplayState State,
		int Turn,
		bool IsPlayerTurn,
		TicTacToePositionElementView.Property[] PositionProperties,
		bool ShowConfirmPositionButton)
	{
		public TicTacToeGameplayProperty(
			TicTacToeGameplayState state) : this(
				state,
				0,
				false,
				new TicTacToePositionElementView.Property[0],
				false) { }
	}

	public interface ITicTacToeGameplayPresenter
	{
		UniTask Run(TicTacToeGameData gameData);
	}

	public class TicTacToeGameplayPresenter : ITicTacToeGameplayPresenter
	{
		private IWarningPresenter _warningPresenter;
		private ITicTacToeGameplayWebSocketPresenter _webSocketPresenter;
		private ITicTacToeGameplayView _view;

		private TicTacToeGameData _gameData;
		private TicTacToeGameplayModel _model;
		private TicTacToeGameplayProperty _prop;
		
		private ActionQueue _actionQueue;

		public const int BOARD_SIZE = 3;

		public TicTacToeGameplayPresenter(
			IWarningPresenter warningPresenter,
			ITicTacToeGameplayWebSocketPresenter webSocketPresenter,
			ITicTacToeGameplayView view)
		{
			_warningPresenter = warningPresenter;
			_webSocketPresenter = webSocketPresenter;
			_view = view;

			_actionQueue = new ActionQueue();

			_view.RegisterCallback(
				(position) =>
					_ChangeStateIfIdle(new TicTacToeGameplayState.ClickPositionElement(position)),
				() =>
					_ChangeStateIfIdle(new TicTacToeGameplayState.ClickPositionConfirmButton()));
		}

		async UniTask ITicTacToeGameplayPresenter.Run(TicTacToeGameData gameData)
		{
			_gameData = gameData;

			_model = new TicTacToeGameplayModel(
				new Observable<int[]>(
					new int[0],
					(positions) => _UpdatePropertyPositions(positions, _model.GhostPositionOpt)),
				new Observable<Option<int>>(
					Option.None<int>(),
					(ghostPositionOpt) => _UpdatePropertyPositions(_model.Positions, ghostPositionOpt)));

			_prop = new TicTacToeGameplayProperty(
				new TicTacToeGameplayState.Open());

			_UpdateModelAndProperty(_gameData);
			await _JoinGame(_gameData.GameId);

			while (_prop.State is not TicTacToeGameplayState.Close)
			{
				_actionQueue.RunAll();
				_view.Render(_prop);
				switch (_prop.State)
				{
					case TicTacToeGameplayState.Open:
						_prop = _prop with { State = new TicTacToeGameplayState.Idle() };
						break;

					case TicTacToeGameplayState.Idle:
						break;

					case TicTacToeGameplayState.ClickPositionElement info:
						if (_model.IsPlayerTurn && _model.Positions[info.Position] == 0)
						{
							_model = _model with
							{
								GhostPositionOpt =
									_model.GhostPositionOpt.Contains(info.Position)
										? Option.None<int>()
										: info.Position.Some(),
							};
						}
						_prop = _prop with 
						{ 
							State = new TicTacToeGameplayState.Idle(),
						};
						break;

					case TicTacToeGameplayState.ClickPositionConfirmButton:
						if (_model.GhostPositionOpt.HasValue)
						{
							var position = _model.GhostPositionOpt.ValueOrFailure();
							_model = _model with
							{
								GhostPositionOpt = Option.None<int>()
							};
							_prop = _prop with 
							{ 
								ShowConfirmPositionButton = false
							};
							_view.Render(_prop);

							await _webSocketPresenter
								.ChoosePosition(position)
								.RunAndHandleInternetError(_warningPresenter);
						}

						_prop = _prop with
						{
							State = new TicTacToeGameplayState.Idle()
						};
						
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

		private async UniTask _JoinGame(int gameId)
		{
			_webSocketPresenter.RegisterOnUpdateGame(result => _actionQueue.Add(() => _UpdateGame(result)));
			_webSocketPresenter.RegisterOnReceiveGameOver(result => _actionQueue.Add(() => _GameOver(result)));

			if (await
				_webSocketPresenter
					.Start(gameId)
					.RunAndHandleInternetError(_warningPresenter)
					.IsFail())
			{
				return;
			}
		}

		private void _UpdateModelAndProperty(TicTacToeGameData gameData)
		{
			_model = _model with
			{
				SelfPlayerId = gameData.SelfPlayerId,
				SelfPlayerTeam = gameData.SelfPlayerTeam,
				IsPlayerTurn = gameData.SelfPlayerTeam == gameData.Board.TurnOfTeam,
				Positions = gameData.Board.Positions,
				GhostPositionOpt = Option.None<int>()
			};

			_prop = _prop with
			{
				Turn = gameData.Board.Turn,
				IsPlayerTurn = _model.IsPlayerTurn,
			};
		}

		private TicTacToePositionElementView.Property[] _GetPositionsProperty(int[] positions, Option<int> ghostPositionOpt)
			=>
				positions
					.Select((value, position) => new TicTacToePositionElementView.Property(
						ghostPositionOpt.Contains(position)
							? _model.SelfPlayerTeam == 1
								? new TicTacToePositionElementView.State.Circle(true)
								: new TicTacToePositionElementView.State.Cross(true)
							: value switch
							{
								0 => new TicTacToePositionElementView.State.Empty(),
								1 => new TicTacToePositionElementView.State.Circle(false),
								2 => new TicTacToePositionElementView.State.Cross(false),
								_ => throw new System.NotImplementedException(),
							}))
					.ToArray();

		private void _UpdatePropertyPositions(int[] positions, Option<int> ghostPositionOpt)
		{
			_prop = _prop with
			{
				PositionProperties = _GetPositionsProperty(positions, ghostPositionOpt),
				ShowConfirmPositionButton = ghostPositionOpt.HasValue
			};
			_view.Render(_prop);
		}

		private void _UpdateGame(TicTacToeGameResult result)
		{
			_UpdateModelAndProperty(new TicTacToeGameData(result, _model.SelfPlayerId));
		}

		private void _GameOver(TicTacToeGameResult result)
		{
			// tbd
		}
	}
}