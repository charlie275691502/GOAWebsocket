using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using Web;
using Common.Warning;
using Cysharp.Threading.Tasks;
using Common.UniTaskExtension;
using Metagame.MainPage;

namespace Metagame
{
	public record MetagameState
	{
		public record MainPage(): MetagameState;
		public record Room(int RoomId): MetagameState;
		public record Close() : MetagameState;
	}

	public record MetagameSubTabReturnType
	{
		public record Switch(MetagameState State) : MetagameSubTabReturnType;
		public record Close() : MetagameSubTabReturnType;
	}

	public record MetagameProperty(MetagameState State);
	public record MetagameSubTabReturn(MetagameSubTabReturnType Type);
	
	public interface IMetagamePresenter : IMainSubTabPresenter
	{

	}
	
	public class MetagamePresenter : IMetagamePresenter
	{
		private IHTTPPresenter _hTTPPresenter;
		private IWarningPresenter _warningPresenter;
		private IMainPagePresenter _mainPagePresneter;
		private IRoomPresenter _roomPresneter;
		private ITopMenuView _topMenuView;
		private BackendPlayerData _backendPlayerData;
		private IRoomWebSocketPresenter _webSocketPresenter;
		
		public MetagamePresenter(
			IHTTPPresenter hTTPPresenter,
			IWarningPresenter warningPresenter,
			IMainPagePresenter mainPagePresneter,
			IRoomPresenter roomPresneter,
			ITopMenuView topMenuView,
			BackendPlayerData backendPlayerData,
			IRoomWebSocketPresenter webSocketPresenter)
		{
			_hTTPPresenter = hTTPPresenter;
			_warningPresenter = warningPresenter;
			_mainPagePresneter = mainPagePresneter;
			_roomPresneter = roomPresneter;
			_topMenuView = topMenuView;
			_backendPlayerData = backendPlayerData;
			_webSocketPresenter = webSocketPresenter;
		}

		async UniTask<MainSubTabReturn> IMainSubTabPresenter.Run()
		{
			await _hTTPPresenter.RefreshSelfPlayerData().RunAndHandleInternetError(_warningPresenter);
			_topMenuView.Enter(_backendPlayerData);

			var prop = new MetagameProperty(new MetagameState.MainPage());
			while (prop.State is not MetagameState.Close)
			{
				var subTabReturn = await (prop.State switch
                {
                    MetagameState.MainPage => _mainPagePresneter.Run(),
                    MetagameState.Room info =>
						_JoinRoom(info.RoomId)
							.Then(_roomPresneter.Run(info.RoomId)),
                    _ => throw new System.NotImplementedException(),
                });

				switch (subTabReturn.Type)
				{
					case MetagameSubTabReturnType.Close:
						prop = prop with { State = new MetagameState.Close() };
						break;

					case MetagameSubTabReturnType.Switch info:
						prop = prop with { State = info.State };
						break;
				}
			}

			_topMenuView.Leave();
			_webSocketPresenter.Stop();
			return new MainSubTabReturn(new MainSubTabReturnType.Switch(new MainState.Game()));
		}
		
		private void _OnLeaveRoom(int roomId)
		{
			_webSocketCommandExecutor.TryAdd(_LeaveRoom(roomId));
		}
		
		private void _OnSendMessage(string message)
		{
			_webSocketPresenter.SendMessage(message);
		}
		
		private async UniTask _JoinRoom(int roomId)
		{
			_webSocketPresenter.RegisterOnReceiveAppendMessage(_OnReceiveAppendMessage);
			_webSocketPresenter.RegisterOnReceiveUpdateRoom(_OnReceiveUpdateRoom);

			if(await 
				_webSocketPresenter
					.Start(roomId)
					.RunAndHandleInternetError(_warningPresenter)
					.IsFail())
			{
				return;
			}

			if (await
				_webSocketPresenter
					.JoinRoom(roomId)
					.RunAndHandleInternetError(_warningPresenter)
					.IsFail())
			{
				return;
			}
		}

		private async UniTask _LeaveRoom(int roomId)
		{
			await _webSocketPresenter
				.LeaveRoom(roomId)
				.RunAndHandleInternetError(_warningPresenter);

			_webSocketPresenter.Stop();
			_roomPresneter.LeaveRoom();
		}
		
		private void _OnReceiveAppendMessage(MessageResult result)
		{
			_roomPresneter.AppendMessage(result);
		}
		
		private void _OnReceiveUpdateRoom(RoomResult result)
		{
			_roomPresneter.UpdateRoom(result);
		}
	}
}
