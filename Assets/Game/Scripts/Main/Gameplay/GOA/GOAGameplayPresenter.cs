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
using Data.Sheet.Container.Sheets;
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
		public record ClickHandCard(int Position) : GOAGameplayState;
		public record ClickUseButton() : GOAGameplayState;
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
		ActionPhase ActionPhase,
		Option<int> SelectingDetailCardIdOpt,
		bool IsGameEnd)
	{
		private bool IsSelfTurn()
			=> TakingTurnPlayerId == SelfPlayerId;
			
		public bool IsSelfTurnAndPhase(Phase phase)
			=> IsSelfTurn() && Board.Phase == phase;
			
		public bool IsSelfTurnAndPhase(Phase phase, ActionPhase actionPhase)
			=> IsSelfTurnAndPhase(phase) && ActionPhase == actionPhase;
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
		bool ShowUseReformHint,
		bool ShowUseExpandHint,
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
				(cardId) =>
					_ChangeStateIfIdle(new GOAGameplayState.ClickHandCard(cardId)),
				() =>
					_ChangeStateIfIdle(new GOAGameplayState.ClickUseButton()),
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
				ActionPhase.None,
				Option.None<int>(),
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
				_GetPublicCardDetailOpt(),
				Option.None<GOAStrategyCardDetaialViewData>(),
				false,
				false,
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
						if (_model.IsSelfTurnAndPhase(Phase.ChooseBoardCardPhase))
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
						} else if (_model.IsSelfTurnAndPhase(Phase.ActionPhase, ActionPhase.Expand))
						{
							if (_model.Board.BoardCards[Info.Position] is CardDataState.Open)
							{
								var card = _model.SelectingDetailCardIdOpt
									.FlatMap(cardId => _googleSheetLoader.Container.GOACards.GetRow(cardId))
									.ValueOrFailure();
								var targetCardId = (_model.Board.BoardCards[Info.Position] as CardDataState.Open).Id;
								var targetCard = _googleSheetLoader.Container.GOACards.GetRow(targetCardId).ValueOrFailure();
								if (!_model.SelectingDetailCardIdOpt.Contains(targetCardId) && card.PowerType == targetCard.PowerType && targetCard.CardType == CardType.Power)
								{
									await _webSocketPresenter
										.UseExpand(_model.SelectingDetailCardIdOpt.ValueOrFailure(), Info.Position)
										.RunAndHandleInternetError(_warningPresenter);
									_model = _model with 
									{ 
										SelectingDetailCardIdOpt = Option.None<int>(),
										ActionPhase = ActionPhase.None,
									};
								}
							}
						}
						
						_UpdatePropertyThenRender();
						_prop = _prop with { State = new GOAGameplayState.Idle() };
						break;
						
					case GOAGameplayState.ClickHandCard Info:
						var cardId = 
							(Info.Position < _model.SelfPlayer.PublicCards.Count() )
								? _model.SelfPlayer.PublicCards[Info.Position]
								: _model.SelfPlayer.StrategyCards[Info.Position - _model.SelfPlayer.PublicCards.Count()];
						if (_model.IsSelfTurnAndPhase(Phase.ActionPhase, ActionPhase.None))
						{
							var card = _googleSheetLoader.Container.GOACards.GetRow(cardId).ValueOrFailure();
							_model = _model with 
							{
								SelectingDetailCardIdOpt = 
									card.CardType == CardType.Power 
										? Option.None<int>()
										: cardId.Some()
							};
						} else if (_model.IsSelfTurnAndPhase(Phase.ActionPhase, ActionPhase.Reform))
						{
							var card = _model.SelectingDetailCardIdOpt
								.FlatMap(cardId => _googleSheetLoader.Container.GOACards.GetRow(cardId))
								.ValueOrFailure();
							var targetCard = _googleSheetLoader.Container.GOACards.GetRow(cardId).ValueOrFailure();
							if (!_model.SelectingDetailCardIdOpt.Contains(cardId) && card.PowerType == targetCard.PowerType && targetCard.CardType == CardType.Power)
							{
								await _webSocketPresenter
									.UseReform(_model.SelectingDetailCardIdOpt.ValueOrFailure(), cardId)
									.RunAndHandleInternetError(_warningPresenter);
								_model = _model with 
								{ 
									SelectingDetailCardIdOpt = Option.None<int>(),
									ActionPhase = ActionPhase.None,
								};
							}
						}
						
						_UpdatePropertyThenRender();
						_prop = _prop with { State = new GOAGameplayState.Idle() };
						break;
						
					case GOAGameplayState.ClickUseButton:
						if (_model.IsSelfTurnAndPhase(Phase.ActionPhase, ActionPhase.None))
						{
							var cardOpt = _model.SelectingDetailCardIdOpt
								.FlatMap(cardId => _googleSheetLoader.Container.GOACards.GetRow(cardId));
								
							if (cardOpt.HasValue)
							{
								switch(cardOpt.ValueOrFailure().CardType)
								{
									case CardType.ActionMask:
										await _webSocketPresenter
											.UseMask(_model.SelectingDetailCardIdOpt.ValueOrFailure())
											.RunAndHandleInternetError(_warningPresenter);
										_model = _model with { SelectingDetailCardIdOpt = Option.None<int>()};
										break;
									case CardType.ActionReform:
										_model = _model with { ActionPhase = ActionPhase.Reform };
										break;
									case CardType.ActionExpand:
										_model = _model with { ActionPhase = ActionPhase.Expand };
										break;
								}
							}
						}
						
						_UpdatePropertyThenRender();
						_prop = _prop with { State = new GOAGameplayState.Idle() };
						break;
						
					case GOAGameplayState.ClickEndTurn:
						if (_model.IsSelfTurnAndPhase(Phase.ActionPhase))
						{
							_model = _model with 
							{
								ActionPhase = ActionPhase.None,
								SelectingDetailCardIdOpt = Option.None<int>(),
							};
							await _webSocketPresenter
								.EndTurn()
								.RunAndHandleInternetError(_warningPresenter);
						}
						
						_UpdatePropertyThenRender();
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
				.Where(order => _GetSelfPlayerModel(gameData).Order != order)
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
								.GetRow(Info.Id)
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
					
		private Option<GOAPublicCardDetaialViewData> _GetPublicCardDetailOpt()
			=> _model.SelectingDetailCardIdOpt
				.FlatMap(cardId => _googleSheetLoader.Container.GOACards.GetRow(cardId))
				.Map(card => card.ImageKey)
				.Map(imageKey => new GOAPublicCardDetaialViewData(
					imageKey,
					_model.IsSelfTurnAndPhase(Phase.ActionPhase, ActionPhase.None)));
					
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
				PublicCardDetailOpt = _GetPublicCardDetailOpt(),
				ShowChooseBoardCardPhaseHint = _model.IsSelfTurnAndPhase(Phase.ChooseBoardCardPhase),
				ShowActionPhaseHint = _model.IsSelfTurnAndPhase(Phase.ActionPhase, ActionPhase.None),
				ShowUseReformHint = _model.IsSelfTurnAndPhase(Phase.ActionPhase, ActionPhase.Reform),
				ShowUseExpandHint = _model.IsSelfTurnAndPhase(Phase.ActionPhase, ActionPhase.Expand),
				ShowEndTurnButton = _model.IsSelfTurnAndPhase(Phase.ActionPhase),
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