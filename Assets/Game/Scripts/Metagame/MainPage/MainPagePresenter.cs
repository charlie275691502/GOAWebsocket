using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.UniTaskExtension;
using Common.Warning;
using Cysharp.Threading.Tasks;
using Optional;
using Optional.Unsafe;
using UnityEngine;
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

		public MainPagePresenter(IHTTPPresenter hTTPPresenter, IWarningPresenter warningPresenter, ICreateRoomPresenter createRoomPresenter, IMainPageView view)
		{
			_hTTPPresenter = hTTPPresenter;
			_warningPresenter = warningPresenter;
			_createRoomPresenter = createRoomPresenter;
			_view = view;

			_view.RegisterCallback(
				(roomId) =>
					_ChangeStateIfIdle(new MainPageState.JoinRoom(roomId)),
				() =>
					_ChangeStateIfIdle(new MainPageState.CreateRoom()));
		}

		async UniTask<MetagameSubTabReturn> IMainPagePresenter.Run()
		{
			var ret = new MetagameSubTabReturn(new MetagameSubTabReturnType.Close());

			var roomListOpt = await _hTTPPresenter.GetRoomList().RunAndHandleInternetError(_warningPresenter);
			if (!roomListOpt.HasValue)
			{
				return ret;
			}

			var roomList = roomListOpt.ValueOrFailure();
			var roomViewDatas = _GetRoomViewDatas(roomList);
			_prop = new MainPageProperty(new MainPageState.Open(), roomViewDatas);

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
								() => _prop = _prop with { State = new MainPageState.Idle() });
						break;

					case MainPageState.Close:
						break;

					default:
						break;
				}
				await UniTask.Yield();
			}

			return ret;
		}

		private void _ChangeStateIfIdle(MainPageState targetState, Action onChangeStateSuccess = null)
		{
			if (_prop.State is not MainPageState.Idle)
				return;

			onChangeStateSuccess?.Invoke();
			_prop = _prop with { State = targetState };
		}
		
		private List<RoomViewData> _GetRoomViewDatas(RoomListResult result)
		{
			return result.Select(roomResult => new RoomViewData()
				{
					Id = roomResult.Id,
					RoomName = roomResult.RoomName,
					GameSetting = new GameSetting(roomResult.GameSetting),
					Players = roomResult.Players.Select(playerDataResult => new PlayerData(playerDataResult)).ToList(),
				}).ToList();
		}

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