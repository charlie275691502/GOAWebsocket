using System;
using System.Collections.Generic;
using System.Linq;
using Common.UniTaskExtension;
using Common.Warning;
using Common.Class;
using Cysharp.Threading.Tasks;
using Optional;
using Optional.Unsafe;
using Web;
using Metagame.MainPage.CreateRoom;

namespace Metagame.MainPage
{
	public interface IMainPagePresenter
	{
		UniTask<MetagameSubTabReturn> Run();
	}

	public abstract record MainPageState
	{
		public record Open() : MainPageState;
		public record Idle() : MainPageState;
		public record RefreshRoomList() : MainPageState;
		public record JoinRoom(int RoomId) : MainPageState;
		public record CreateRoom() : MainPageState;
		public record Close() : MainPageState;
	}

	public record MainPageProperty(MainPageState State, List<RoomViewData> Rooms);

	public class MainPagePresenter : IMainPagePresenter
	{
		private IHTTPPresenter _hTTPPresenter;
		private IWarningPresenter _warningPresenter;
		private ICreateRoomPresenter _createRoomPresenter;
		private IMainPageView _view;
		
		private MainPageProperty _prop;

		public MainPagePresenter(
			IHTTPPresenter hTTPPresenter,
			IWarningPresenter warningPresenter,
			ICreateRoomPresenter createRoomPresenter,
			IMainPageView view)
		{
			_hTTPPresenter = hTTPPresenter;
			_warningPresenter = warningPresenter;
			_createRoomPresenter = createRoomPresenter;
			_view = view;

			_view.RegisterCallback(
				() =>
					_ChangeStateIfIdle(new MainPageState.RefreshRoomList()),
				(roomId) =>
					_ChangeStateIfIdle(new MainPageState.JoinRoom(roomId)),
				() =>
					_ChangeStateIfIdle(new MainPageState.CreateRoom()));
		}

		async UniTask<MetagameSubTabReturn> IMainPagePresenter.Run()
		{
			var ret = new MetagameSubTabReturn(new MetagameSubTabReturnType.Close());

			await _GetRoomList()
				.Match(
					rooms => 
						_prop = new MainPageProperty(new MainPageState.Open(), rooms),
					() => 
						_prop = new MainPageProperty(new MainPageState.Close(), new List<RoomViewData>()));

			while (_prop.State is not MainPageState.Close)
			{
				_view.Render(_prop);
				switch (_prop.State)
				{
					case MainPageState.Open:
						_prop = _prop with { State = new MainPageState.Idle() };
						break;

					case MainPageState.Idle:
						break;

					case MainPageState.RefreshRoomList:
						await _GetRoomList()
							.Match(
								rooms => 
									_prop = _prop with 
									{ 
										State = new MainPageState.Idle(),
										Rooms = rooms,
									},
								() => 
									_prop = new MainPageProperty(new MainPageState.Close(), new List<RoomViewData>()));
						break;

					case MainPageState.JoinRoom info:
						_prop = _prop with { State = new MainPageState.Close() };
						ret = new MetagameSubTabReturn(new MetagameSubTabReturnType.Switch(new MetagameState.Room(info.RoomId)));
						break;

					case MainPageState.CreateRoom:
						await _CreateRoom()
							.Match(
								roomId =>
								{
									_prop = _prop with { State = new MainPageState.Close() };
									ret = new MetagameSubTabReturn(new MetagameSubTabReturnType.Switch(new MetagameState.Room(roomId)));
								},
								() => 
									_prop = _prop with { State = new MainPageState.Idle() });
						break;

					case MainPageState.Close:
						break;

					default:
						break;
				}
				await UniTask.Yield();
			}

			_view.Render(_prop);
			return ret;
		}

		private void _ChangeStateIfIdle(MainPageState targetState, Action onChangeStateSuccess = null)
		{
			if (_prop.State is not MainPageState.Idle)
				return;

			onChangeStateSuccess?.Invoke();
			_prop = _prop with { State = targetState };
		}
		
		private async UniTask<Option<List<RoomViewData>>> _GetRoomList()
		{
			var roomListOpt = await _hTTPPresenter.GetRoomList().RunAndHandleInternetError(_warningPresenter);
			if (!roomListOpt.HasValue)
			{
				return Option.None<List<RoomViewData>>();
			}

			var roomList = roomListOpt.ValueOrFailure();
			return _GetRoomViewDatas(roomList).Some();
		}
		
		private List<RoomViewData> _GetRoomViewDatas(RoomListResult result)
			=> result
				.Where(roomResult => roomResult.Players.Count > 0)
				.Select(roomResult => 
					new RoomViewData(
						roomResult.Id,
						roomResult.RoomName,
						new GameSetting(roomResult.GameSetting),
						roomResult.Players.Select(playerDataResult => new PlayerViewData(playerDataResult)).ToList()
					))
				.ToList();

		private async UniTask<Option<int>> _CreateRoom()
		{
			var createRoomReturn = await _createRoomPresenter.Run();
			switch (createRoomReturn.Type)
			{
				case CreateRoomReturnType.Confirm info:
					return await _hTTPPresenter
						.CreateRoom(
							info.RoomName,
							GameTypeUtility.GetAbbreviation(info.GameType),
							info.PlayerPlot)
						.RunAndHandleInternetError(_warningPresenter)
						.Map(roomResult => roomResult.Id);
				case CreateRoomReturnType.Cancel:
					return Option.None<int>();
			}
			return Option.None<int>();
		}
	}
}