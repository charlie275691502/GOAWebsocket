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

namespace Gameplay.GOA
{
	public abstract record GOAGameplayState
	{
		public record Open() : GOAGameplayState;
		public record Idle() : GOAGameplayState;
		public record ClickPositionElement(int Position) : GOAGameplayState;
		public record ClickReturnHome() : GOAGameplayState;
		public record Close() : GOAGameplayState;
	}

	public record GOAGameplayModel(
		int SelfPlayerId,
		int SelfPlayerTeam,
		bool IsPlayerTurn,
		bool IsGameEnd)
	{
		public GOAGameplayModel(
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

	public record GOAGameplayProperty(
		GOAGameplayState State,
		int Turn,
		bool IsPlayerTurn,
		PlayerViewData[] PlayerViewDatas,
		bool ShowConfirmPositionButton,
		bool IsGameEnd,
		string WinnerName,
		int SummaryTurns)
	{
		public GOAGameplayProperty(
			GOAGameplayState state, PlayerViewData[] playerViewDatas) : this(
				state,
				0,
				false,
				playerViewDatas,
				false,
				false,
				string.Empty,
				0) { }
	}

	public interface IGOAGameplayPresenter
	{
		UniTask Run(GOAGameData gameData);
	}

	public class GOAGameplayPresenter : IGOAGameplayPresenter
	{
		private IWarningPresenter _warningPresenter;
		private IGOAGameplayWebSocketPresenter _webSocketPresenter;
		private IGOAGameplayView _view;

		private GOAGameData _gameData;
		private GOAGameplayModel _model;
		private GOAGameplayProperty _prop;
		
		private ActionQueue _actionQueue;

		public const int BOARD_SIZE = 3;

		public GOAGameplayPresenter(
			DiContainer container,
			IWarningPresenter warningPresenter,
			IGOAGameplayWebSocketPresenter webSocketPresenter,
			IGOAGameplayView view)
		{
			_warningPresenter = warningPresenter;
			_webSocketPresenter = webSocketPresenter;
			_view = view;

			_actionQueue = new ActionQueue();

			_view.RegisterCallback(
				container,
				(position) =>
					_ChangeStateIfIdle(new GOAGameplayState.ClickPositionElement(position)),
				() =>
					_ChangeStateIfIdle(new GOAGameplayState.ClickReturnHome()));
		}

		async UniTask IGOAGameplayPresenter.Run(GOAGameData gameData)
		{
			_gameData = gameData;

			_model = new GOAGameplayModel(
				new Observable<int[]>(
					new int[0],
					(positions) => _UpdatePropertyPositions(positions, _model.GhostPositionOpt)),
				new Observable<Option<int>>(
					Option.None<int>(),
					(ghostPositionOpt) => _UpdatePropertyPositions(_model.Positions, ghostPositionOpt)));

			_prop = new GOAGameplayProperty(
				new GOAGameplayState.Open(),
				_gameData.Players.Select(player => player.Player).ToArray());

			_UpdateModelAndProperty(_gameData);
			await _JoinGame(_gameData.GameId);

			while (_prop.State is not GOAGameplayState.Close)
			{
				_actionQueue.RunAll();
				_view.Render(_prop);
				switch (_prop.State)
				{
					case GOAGameplayState.Open:
						_prop = _prop with { State = new GOAGameplayState.Idle() };
						break;

					case GOAGameplayState.Idle:
						break;

					case GOAGameplayState.ClickPositionElement info:
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
							State = new GOAGameplayState.Idle(),
						};
						break;
						
					case GOAGameplayState.ClickReturnHome:

						_prop = _prop with
						{
							State = new GOAGameplayState.Close()
						};
						
						break;

					case GOAGameplayState.Close:
						break;

					default:
						break;
				}
				await UniTask.Yield();
			}

			_LeaveGame();
			_view.Render(_prop);
		}

		private void _ChangeStateIfIdle(GOAGameplayState targetState, Action onChangeStateSuccess = null)
		{
			if (_prop.State is not GOAGameplayState.Idle)
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

		private void _UpdateModelAndProperty(GOAGameData gameData)
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

		private void _UpdatePropertyPositions(int[] positions, Option<int> ghostPositionOpt)
		{
			_prop = _prop with
			{
				ShowConfirmPositionButton = ghostPositionOpt.HasValue
			};
			_view.Render(_prop);
		}

		private void _UpdateGame(GOAGameResult result)
		{
			_UpdateModelAndProperty(new GOAGameData(result, _model.SelfPlayerId));
		}

		private void _ReceiveSummary(GOASummaryResult result)
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