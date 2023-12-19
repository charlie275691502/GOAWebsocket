using Web;
using System.Linq;
using System;
using Common;
using Common.Warning;
using Common.Class;
using Common.UniTaskExtension;
using Cysharp.Threading.Tasks;
using Optional.Unsafe;
using Data.Sheet;

namespace Metagame.Room
{
	public abstract record RoomState
	{
		public record Open() : RoomState;
		public record Idle() : RoomState;
		public record SendMessage(string Message) : RoomState;
		public record SendStartGame() : RoomState;
		public record StartGame(int GameId) : RoomState;
		public record Leave() : RoomState;
		public record Close() : RoomState;
	}

	public record RoomProperty(RoomState State, RoomWithMessagesViewData Room);

	public interface IRoomPresenter
	{
		UniTask<MetagameSubTabReturn> Run(int roomId);
	}

	public class RoomPresenter : IRoomPresenter
	{
		private IHTTPPresenter _hTTPPresenter;
		private IWarningPresenter _warningPresenter;
		private IRoomWebSocketPresenter _webSocketPresenter;
		private IRoomView _view;
		private IExcelDataSheetLoader _excelDataSheetLoader;

		private ActionQueue _actionQueue;

		private RoomProperty _prop;

		public RoomPresenter(
			IHTTPPresenter hTTPPresenter,
			IRoomWebSocketPresenter webSocketPresenter,
			IWarningPresenter warningPresenter,
			IRoomView view,
			IExcelDataSheetLoader excelDataSheetLoader)
		{
			_hTTPPresenter = hTTPPresenter;
			_webSocketPresenter = webSocketPresenter;
			_warningPresenter = warningPresenter;
			_view = view;
			_excelDataSheetLoader = excelDataSheetLoader;

			_actionQueue = new ActionQueue();
			
			_view.RegisterCallback(
				() =>
					_ChangeStateIfIdle(new RoomState.Leave()),
				(message) =>
					_ChangeStateIfIdle(new RoomState.SendMessage(message)),
				() =>
					_ChangeStateIfIdle(new RoomState.SendStartGame()));
		}

		async UniTask<MetagameSubTabReturn> IRoomPresenter.Run(int roomId)
		{
			await _JoinRoom(roomId);
			
			var ret = new MetagameSubTabReturn(new MetagameSubTabReturnType.Close());

			var roomWithMessagesOpt = await _hTTPPresenter.GetRoomWithMessages(roomId).RunAndHandleInternetError(_warningPresenter);
			if (!roomWithMessagesOpt.HasValue)
			{
				return ret;
			}

			_prop = new RoomProperty(new RoomState.Open(), _GetRoomWithMessagesViewData(roomWithMessagesOpt.ValueOrFailure()));

			while (_prop.State is not RoomState.Close)
			{
				_actionQueue.RunAll();
				_view.Render(_prop);
				switch (_prop.State)
				{
					case RoomState.Open:
						_prop = _prop with { State = new RoomState.Idle() };
						break;

					case RoomState.Idle:
						break;

					case RoomState.SendMessage info:
						_webSocketPresenter.SendMessage(info.Message);
						_prop = _prop with { State = new RoomState.Idle() };
						break;

					case RoomState.SendStartGame:
						_prop = _prop with { Room = _prop.Room with { EnableStartGameButton = false } };
						_view.Render(_prop);
						await
						   _webSocketPresenter
							   .SendStartGame()
							   .RunAndHandleInternetError(_warningPresenter);
						_prop = _prop with { State = new RoomState.Idle(), Room = _prop.Room with { EnableStartGameButton = true } };
						_view.Render(_prop);
						break;

					case RoomState.StartGame info:
						_LeaveRoom();
						ret = ret with { Type = new MetagameSubTabReturnType.Switch(new MetagameState.Game(_prop.Room.GameSetting.GameType, info.GameId)) };
						_prop = _prop with { State = new RoomState.Close() };
						break;

					case RoomState.Leave:
						_LeaveRoom();
						ret = ret with { Type = new MetagameSubTabReturnType.Switch(new MetagameState.MainPage()) };
						_prop = _prop with { State = new RoomState.Close() };
						break;

					case RoomState.Close:
						break;

					default:
						break;
				}
				await UniTask.Yield();
			}

			_view.Render(_prop);
			return ret;
		}

		private void _ChangeStateIfIdle(RoomState targetState, Action onChangeStateSuccess = null)
		{
			if (_prop.State is not RoomState.Idle)
				return;

			onChangeStateSuccess?.Invoke();
			_prop = _prop with { State = targetState };
		}

		private RoomWithMessagesViewData _GetRoomWithMessagesViewData(RoomWithMessagesResult result)
		{
			return new RoomWithMessagesViewData(
				result.Id,
				result.RoomName,
				new GameSetting(result.GameSetting),
				result.Players.Select(playerDataResult => new PlayerViewData(playerDataResult, _excelDataSheetLoader)).ToList(),
				result.Messages.Select(message => new MessageViewData()
				{
					Id = message.Id,
					Content = message.Content,
					NickName = message.Player.NickName,
					AvatarImageKey =
						_excelDataSheetLoader.Container.Avatars
							.GetRow(message.Player.AvatarId)
							.Map(avatar => avatar.ImageKey)
							.ValueOr(string.Empty),
				}).ToList(),
				true
			);
		}

		private async UniTask _JoinRoom(int roomId)
		{
			_webSocketPresenter.RegisterOnReceiveAppendMessage(result => _actionQueue.Add(() => _AppendMessage(result)));
			_webSocketPresenter.RegisterOnReceiveUpdateRoom(result => _actionQueue.Add(() => _UpdateRoom(result)));
			_webSocketPresenter.RegisterOnReceiveStartGame(result => _actionQueue.Add(() => _StartGame(result)));

			if (await
				_webSocketPresenter
					.Start(roomId)
					.RunAndHandleInternetError(_warningPresenter)
					.IsFail())
			{
				return;
			}
			
			if (await
				_webSocketPresenter
					.JoinRoom()
					.RunAndHandleInternetError(_warningPresenter)
					.IsFail())
			{
				return;
			}
		}

		private void _LeaveRoom()
		{
			_webSocketPresenter.Stop();
		}

		private void _AppendMessage(MessageResult result)
		{
			_prop = _prop with
			{
				Room = _prop.Room with
				{
					Messages = _prop.Room.Messages
						.Append(new MessageViewData()
						{
							Id = result.Id,
							Content = result.Content,
							NickName = result.Player.NickName,
							AvatarImageKey =
								_excelDataSheetLoader.Container.Avatars
									.GetRow(result.Player.AvatarId)
									.Map(avatar => avatar.ImageKey)
									.ValueOr(string.Empty),
						})
						.ToList()
				}
			};
		}

		private void _UpdateRoom(RoomResult result)
		{
			_prop = _prop with
			{
				Room = _prop.Room with
				{
					Id = result.Id,
					RoomName = result.RoomName,
					GameSetting = new GameSetting(result.GameSetting),
					Players = result.Players.Select(playerDataResult => new PlayerViewData(playerDataResult, _excelDataSheetLoader)).ToList()
				}
			};
		}

		private void _StartGame(TicTacToeGameResult result)
		{
			_prop = _prop with { State = new RoomState.StartGame(result.Id) };
		}
	}
}