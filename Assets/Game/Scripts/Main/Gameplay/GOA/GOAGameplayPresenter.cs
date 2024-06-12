using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common;
using Common.AssetSession;
using Common.LinqExtension;
using Common.Observable;
using Common.UniTaskExtension;
using Common.Warning;
using Cysharp.Threading.Tasks;
using Data.Sheet;
using Metagame;
using ModestTree;
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
		public record ClickBoardCard(int Position) : GOAGameplayState;
		public record ClickEndTurn() : GOAGameplayState;
		public record Close() : GOAGameplayState;
	}

	public record GOAGameplayModel(
		int SelfPlayerId,
		int TakingTurnPlayerId,
		GOAPlayerData SelfPlayer,
		GOAPlayerData[] EnemyPlayers,
		GOABoardData Board,
		List<int> SelectingBoardCards,
		List<int> SelectingHandCards,
		bool IsGameEnd)
		{
			public bool IsSelfTurn()
				=> TakingTurnPlayerId == SelfPlayerId;
		}
		

	public record GOAGameplayProperty(
		GOAGameplayState State,
		int Turn,
		GOAPlayerViewData SelfPlayer,
		GOAPlayerViewData[] EnemyPlayers,
		GOABoardViewData Board,
		GOAHandCardsViewData HandPublicCards,
		Option<GOACharacterDetailViewData> CharacterDetailOpt,
		Option<GOAPublicCardDetaialViewData> PublicCardDetailOpt,
		Option<GOAStrategyCardDetaialViewData> StrategyCardDetailOpt,
		bool ShowChooseBoardCardPhaseHint,
		bool ShowActionPhaseHint,
		bool ShowEndTurnButton);

	public interface IGOAGameplayPresenter
	{
		UniTask Run(GOAGameData gameData);
	}

	public class GOAGameplayPresenter : IGOAGameplayPresenter
	{
		private IWarningPresenter _warningPresenter;
		private IGOAGameplayWebSocketPresenter _webSocketPresenter;
		private IGoogleSheetLoader _googleSheetLoader;
		private IGOAGameplayView _view;

		private GOAGameData _gameData;
		private GOAGameplayModel _model;
		private GOAGameplayProperty _prop;
		
		private ActionQueue _actionQueue;

		public const int BOARD_SIZE = 3;

		public GOAGameplayPresenter(
			IAssetSession assetSession,
			IWarningPresenter warningPresenter,
			IGOAGameplayWebSocketPresenter webSocketPresenter,
			IGoogleSheetLoader googleSheetLoader,
			IGOAGameplayView view)
		{
			_warningPresenter = warningPresenter;
			_webSocketPresenter = webSocketPresenter;
			_googleSheetLoader = googleSheetLoader;
			_view = view;

			_actionQueue = new ActionQueue();

			_view.RegisterCallback(
				assetSession,
				(position) =>
					_ChangeStateIfIdle(new GOAGameplayState.ClickBoardCard(position)),
				() =>
					_ChangeStateIfIdle(new GOAGameplayState.ClickEndTurn()));
		}

		async UniTask IGOAGameplayPresenter.Run(GOAGameData gameData)
		{
			_gameData = gameData;

			_model = new GOAGameplayModel(
				gameData.SelfPlayerId,
				gameData.Board.TakingTurnPlayerId,
				_GetSelfPlayerModel(gameData),
				_GetEnemyPlayersModel(gameData),
				gameData.Board,
				new List<int>(),
				new List<int>(),
				false
			);
			
			_prop = new GOAGameplayProperty(
				new GOAGameplayState.Open(),
				_model.Board.Turn,
				_GetSelfPlayerViewData(),
				_GetEnemyPlayersViewData(),
				_GetBoardViewData(),
				_GetHandCardsViewData(),
				Option.None<GOACharacterDetailViewData>(),
				Option.None<GOAPublicCardDetaialViewData>(),
				Option.None<GOAStrategyCardDetaialViewData>(),
				false,
				false,
				false);

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
						
					case GOAGameplayState.ClickBoardCard Info:
						if (_model.IsSelfTurn() && _model.Board.Phase == Phase.ChooseBoardCardPhase)
						{
							if (_model.Board.BoardCards[Info.Position] is CardDataState.Covered && _model.Board.RevealingBoardCardPositions.Count() == 0)
							{
								if (_model.SelectingBoardCards.Contains(Info.Position))
									_model.SelectingBoardCards.Remove(Info.Position);
								else 
									_model.SelectingBoardCards.Add(Info.Position);
									
								if (_model.SelectingBoardCards.Count() >= 2)
								{
									await _webSocketPresenter
										.RevealBoardCards(_model.SelectingBoardCards.ToArray())
										.RunAndHandleInternetError(_warningPresenter);
									_model.SelectingBoardCards.Clear();
								}
							} else if (_model.Board.BoardCards[Info.Position] is CardDataState.Open)
							{
								if (_model.Board.RevealingBoardCardPositions.ContainsItem(Info.Position))
								{
									await _webSocketPresenter
										.ChooseRevealingBoardCard(Info.Position)
										.RunAndHandleInternetError(_warningPresenter);
								} else 
								{
									await _webSocketPresenter
										.ChooseOpenBoardCard(Info.Position)
										.RunAndHandleInternetError(_warningPresenter);
								}
								_model.SelectingBoardCards.Clear();
							}
							
							_UpdatePropertyThenRender();
						}
						
						_prop = _prop with { State = new GOAGameplayState.Idle() };
						break;
						
					case GOAGameplayState.ClickEndTurn:
						if (_model.IsSelfTurn() && _model.Board.Phase == Phase.ActionPhase)
						{
							await _webSocketPresenter
								.EndTurn()
								.RunAndHandleInternetError(_warningPresenter);
						}
						_prop = _prop with { State = new GOAGameplayState.Idle() };
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
		
		private int[] _GetEnemyPlayerOrders(GOAGameData gameData)
		{
			var selfPlayerOrder = gameData.Players
				.FirstOrNone(player => player.Player.Id == gameData.SelfPlayerId)
				.Map(player => player.Order)
				.ValueOrFailure();
			var playerCount = gameData.Players.Count();
			return Enumerable.Range(1, playerCount)
				.Select(index => (selfPlayerOrder + index) % playerCount)
				.ToArray();
		}
		
		private GOAPlayerData _GetSelfPlayerModel(GOAGameData gameData)
			=> gameData.Players
				.FirstOrNone(player => player.Player.Id == gameData.SelfPlayerId)
				.ValueOrFailure();
				
		private GOAPlayerData[] _GetEnemyPlayersModel(GOAGameData gameData)
			=> _GetEnemyPlayerOrders(gameData)
				.Select(order => gameData.Players.FirstOrNone(player => player.Player.Id == gameData.SelfPlayerId))
				.Values()
				.ToArray();
		
		private GOAPlayerViewData _GetSelfPlayerViewData()
			=> new GOAPlayerViewData(
				_model.SelfPlayer,
				true,
				_model.Board.TakingTurnPlayerId,
					_googleSheetLoader);

		private GOAPlayerViewData[] _GetEnemyPlayersViewData()
			=> _model.EnemyPlayers
				.Select(enemyPlayer => new GOAPlayerViewData(
					enemyPlayer,
					false,
					_model.Board.TakingTurnPlayerId,
					_googleSheetLoader))
				.ToArray();
		private GOABoardViewData _GetBoardViewData()
			=> new GOABoardViewData(
				_model.Board.DrawCardCount,
				_model.Board.GraveCardCount,
				_model.Board.BoardCards
					.Select<CardDataState, CardViewDataState>((state, position) => state switch
					{
						CardDataState.Empty => new CardViewDataState.Empty(),
						CardDataState.Covered => new CardViewDataState.Covered(_model.SelectingBoardCards.Contains(position)),
						CardDataState.Open Info => new CardViewDataState.Open(
							true,
							_model.Board.RevealingBoardCardPositions.Contains(position),
							_googleSheetLoader.Container.GOACards
								.GetRow(Info.Key)
								.Map(card => card.ImageKey)
								.ValueOr(string.Empty)),
						_ => throw new NotImplementedException(),
					})
					.Select(state => new GOACardViewData(state))
					.ToArray());
		private GOAHandCardsViewData _GetHandCardsViewData()
			=> new GOAHandCardsViewData(
				_model.SelfPlayer.PublicCards
					.Select(cardKey => new GOACardViewData(new CardViewDataState.Open(
						true,
						false,
						_googleSheetLoader.Container.GOACards
							.GetRow(cardKey)
							.Map(card => card.ImageKey)
							.ValueOr(string.Empty))))
					.Concat(
						_model.SelfPlayer.StrategyCards
							.Select(cardKey => new GOACardViewData(new CardViewDataState.Open(
								false,
								false,
								_googleSheetLoader.Container.GOACards
									.GetRow(cardKey)
									.Map(card => card.ImageKey)
									.ValueOr(string.Empty)))))
					.ToArray());
					
		private void _UpdateModel(GOAGameData gameData)
		{
			_model = _model with
			{
				TakingTurnPlayerId = gameData.Board.TakingTurnPlayerId,
				SelfPlayer = _GetSelfPlayerModel(gameData),
				EnemyPlayers = _GetEnemyPlayersModel(gameData),
				Board = gameData.Board,
			};
		}
					
		private void _UpdatePropertyThenRender()
		{
			_prop = _prop with
			{
				Turn = _model.Board.Turn,
				SelfPlayer = _GetSelfPlayerViewData(),
				EnemyPlayers = _GetEnemyPlayersViewData(),
				Board = _GetBoardViewData(),
				HandPublicCards = _GetHandCardsViewData(),
				ShowChooseBoardCardPhaseHint = _model.IsSelfTurn() && _model.Board.Phase == Phase.ChooseBoardCardPhase,
				ShowActionPhaseHint = _model.IsSelfTurn() && _model.Board.Phase == Phase.ActionPhase,
				ShowEndTurnButton = _model.IsSelfTurn() && _model.Board.Phase == Phase.ActionPhase,
			};
			
			_view.Render(_prop);
		}

		private async UniTask _JoinGame(int gameId)
		{
			_webSocketPresenter.RegisterOnUpdateGame(result => _actionQueue.Add(() => _UpdateGame(result)));

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

		private void _UpdateGame(GOAGameResult result)
		{
			_UpdateModel(new GOAGameData(result, _model.SelfPlayerId, _googleSheetLoader));
			_UpdatePropertyThenRender();
		}
	}
}