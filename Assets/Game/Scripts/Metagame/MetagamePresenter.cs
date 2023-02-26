using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rayark.Mast;
using Common;

namespace Metagame
{
	public enum MetagameTabResult
	{
		ToMainPage,
		ToRoom,
		EnterGame,
	}
	
	public interface IMetagamePresenter
	{
		IEnumerator Run();
	}
	
	public class MetagamePresenter : IMetagamePresenter
	{
		private enum Tab
		{
			MainPage,
			Room,
		}
		
		private IMainPagePresenter _mainPagePresneter;
		private IRoomPresenter _roomPresneter;
		private ITopMenuView _topMenuView;
		private BackendPlayerData _backendPlayerData;
		
		public MetagamePresenter(
			IMainPagePresenter mainPagePresneter,
			IRoomPresenter roomPresneter,
			ITopMenuView topMenuView,
			BackendPlayerData backendPlayerData)
		{
			_mainPagePresneter = mainPagePresneter;
			_roomPresneter = roomPresneter;
			_topMenuView = topMenuView;
			_backendPlayerData = backendPlayerData;
		}
		
		public IEnumerator Run()
		{
			yield return _Run();
		}
		
		private IEnumerator _Run()
		{
			_topMenuView.Enter(_backendPlayerData);
			
			var nowTab = Tab.MainPage;
			while (true)
			{
				var monad = 
					(nowTab == Tab.MainPage) 
						? new BlockMonad<MetagameTabResult>(r => _mainPagePresneter.Run(r))
						: new BlockMonad<MetagameTabResult>(r => _roomPresneter.Run(r));
				yield return monad.Do();
				if (monad.Error != null)
				{
					Debug.LogError(monad.Error);
					break;
				}
				
				if (monad.Result == MetagameTabResult.ToRoom)
				{
					break;
				}
				
				nowTab = 
					(monad.Result == MetagameTabResult.ToMainPage) 
						? Tab.MainPage
						: Tab.Room;
			}
		}
	}
}
