using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using Web;
using Common.Warning;
using Cysharp.Threading.Tasks;
using Common.UniTaskExtension;
using Metagame.MainPage;
using Metagame.Room;

namespace Metagame
{
	public record MetagameState
	{
		public record MainPage(): MetagameState;
		public record Room(int RoomId): MetagameState;
		public record Close() : MetagameState;
	}

	public record MetagameProperty(MetagameState State);
	
	public interface IMetagamePresenter : IMainSubTabPresenter
	{

	}
	
	public class MetagamePresenter : IMetagamePresenter
	{
		private IHTTPPresenter _hTTPPresenter;
		private IWarningPresenter _warningPresenter;
		private IMainPagePresenter _mainPagePresneter;
		private IRoomPresenter _roomPresenter;
		private ITopMenuView _topMenuView;
		private BackendPlayerData _backendPlayerData;
		private IRoomWebSocketPresenter _webSocketPresenter;
		
		public MetagamePresenter(
			IHTTPPresenter hTTPPresenter,
			IWarningPresenter warningPresenter,
			IMainPagePresenter mainPagePresneter,
			IRoomPresenter roomPresenter,
			ITopMenuView topMenuView,
			BackendPlayerData backendPlayerData,
			IRoomWebSocketPresenter webSocketPresenter)
		{
			_hTTPPresenter = hTTPPresenter;
			_warningPresenter = warningPresenter;
			_mainPagePresneter = mainPagePresneter;
			_roomPresenter = roomPresenter;
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
                    MetagameState.Room info => _roomPresenter.Run(info.RoomId),
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
	}
}
