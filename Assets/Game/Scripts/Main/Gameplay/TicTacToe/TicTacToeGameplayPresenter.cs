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
using Metagame;
using Optional;
using Optional.Collections;
using Optional.Unsafe;
using Web;
using Zenject;

namespace Gameplay.TicTacToe
{
	public abstract record TicTacToeGameplayState
	{
		public record Open() : TicTacToeGameplayState;
		public record Idle() : TicTacToeGameplayState;
		public record ClickPositionElement(int Position) : TicTacToeGameplayState;
		public record ClickReturnHome() : TicTacToeGameplayState;
		public record Close() : TicTacToeGameplayState;
	}

	public record TicTacToeGameplayModel(
		int SelfPlayerId,
		int SelfPlayerTeam,
		bool IsPlayerTurn,
		bool IsGameEnd)
	{
		public TicTacToeGameplayModel(
			Observable<int[]> positionsObs,
			Observable<Option<int>> ghostPositionObs) : this(
				0,
				0,
				false,
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
		PlayerViewData[] PlayerViewDatas,
		bool ShowConfirmPositionButton,
		bool IsGameEnd,
		string WinnerName,
		int SummaryTurns)
	{
		public TicTacToeGameplayProperty(
			TicTacToeGameplayState state, PlayerViewData[] playerViewDatas) : this(
				state,
				0,
				false,
				new TicTacToePositionElementView.Property[0],
				playerViewDatas,
				false,
				false,
				string.Empty,
				0) { }
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
			DiContainer container,
			IWarningPresenter warningPresenter,
			ITicTacToeGameplayWebSocketPresenter webSocketPresenter,
			ITicTacToeGameplayView view)
		{
			_warningPresenter = warningPresenter;
			_webSocketPresenter = webSocketPresenter;
			_view = view;

			_actionQueue = new ActionQueue();

			_view.RegisterCallback(
				container,
				(position) =>
					_ChangeStateIfIdle(new TicTacToeGameplayState.ClickPositionElement(position)),
				() =>
					_ChangeStateIfIdle(new TicTacToeGameplayState.ClickReturnHome()));
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
				new TicTacToeGameplayState.Open(),
				_gameData.Players.Select(player => player.Player).ToArray());

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
						if (
							!_model.IsGameEnd &&
							_model.IsPlayerTurn &&
							_model.Positions[info.Position] == 0)
						{
							if (_model.GhostPositionOpt.Contains(info.Position))
							{
								if (
									!_model.IsGameEnd &&
									_model.GhostPositionOpt.HasValue)
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
								} else 
								{
									_model = _model with
									{
										GhostPositionOpt = Option.None<int>()
									};
								}
							} else 
							{
								_model = _model with
								{
									GhostPositionOpt = info.Position.Some()
								};
							}
						}
						_prop = _prop with 
						{ 
							State = new TicTacToeGameplayState.Idle(),
						};
						break;
						
					case TicTacToeGameplayState.ClickReturnHome:

						_prop = _prop with
						{
							State = new TicTacToeGameplayState.Close()
						};
						
						break;

					case TicTacToeGameplayState.Close:
						break;

					default:
						break;
				}
				await UniTask.Yield();
			}

			_LeaveGame();
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
			_webSocketPresenter.RegisterOnReceiveSummary(result => _actionQueue.Add(() => _ReceiveSummary(result)));

			if (await
				_webSocketPresenter
					.Start(gameId)
					.RunAndHandleInternetError(_warningPresenter)
					.IsFail())
			{
				return;
			}
		}

		private void _LeaveGame()
		{
			_webSocketPresenter.Stop();
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

		private void _ReceiveSummary(TicTacToeSummaryResult result)
		{
			_model = _model with 
			{
				IsGameEnd = true,
			};

			_prop = _prop with
			{
				IsGameEnd = true,
				WinnerName = result.Winner.Player.NickName,
				SummaryTurns = result.Turns,
			};
			
			_view.Render(_prop);
		}
	}
}