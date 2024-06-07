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
		bool IsGameEnd);

	public record GOAGameplayProperty(
		GOAGameplayState State,
		int Turn,
		GOAPlayerViewData SelfPlayer,
		GOAPlayerViewData[] EnemyPlayers,
		GOABoardViewData Board,
		GOAHandCardsViewData HandPublicCards,
		Option<GOACharacterDetailViewData> CharacterDetailOpt,
		Option<GOAPublicCardDetaialViewData> PublicCardDetailOpt,
		Option<GOAStrategyCardDetaialViewData> StrategyCardDetailOpt);

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

			_view.RegisterCallback(assetSession);
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
				Option.None<GOAStrategyCardDetaialViewData>());

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
				_model.Board.TakingTurnPlayerId);

		private GOAPlayerViewData[] _GetEnemyPlayersViewData()
			=> _model.EnemyPlayers
				.Select(enemyPlayer => new GOAPlayerViewData(
					enemyPlayer,
					false,
					_model.Board.TakingTurnPlayerId))
				.ToArray();
		private GOABoardViewData _GetBoardViewData()
			=> new GOABoardViewData(
				_model.Board.DrawCardCount,
				_model.Board.GraveCardCount,
				_model.Board.Cards
					.Select<CardDataState, CardViewDataState>(state => state switch
					{
						CardDataState.Empty => new CardViewDataState.Empty(),
						CardDataState.Covered => new CardViewDataState.Covered(false),
						CardDataState.Open Info => new CardViewDataState.Open(false, false, string.Empty),
						_ => throw new NotImplementedException(),
					})
					.Select(state => new GOACardViewData(state))
					.ToArray());
		private GOAHandCardsViewData _GetHandCardsViewData()
			=> new GOAHandCardsViewData(
				_model.SelfPlayer.PublicCardIds
					.Select(cardId => new GOACardViewData(new CardViewDataState.Open(false, false, cardId)))
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