using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rayark.Mast;
using Common;
using Web;

namespace Metagame
{
	public enum MetagameStatusType
	{
		MainPage,
		Room,
		EnterGame,
	}
	
	public class MetagameStatus
	{
		public MetagameStatusType Type;
		public int ToRoomId;
		
		public MetagameStatus(MetagameStatusType type)
		{
			Type = type;
		}
		
		public MetagameStatus(MetagameStatusType type, int roomId)
		{
			Type = type;
			ToRoomId = roomId;
		}
	}
	
	public interface IMetagamePresenter
	{
		IEnumerator Run();
	}
	
	public class MetagamePresenter : IMetagamePresenter
	{
		private IHTTPPresenter _hTTPPresenter;
		private IWarningPresenter _warningPresenter;
		private IMainPagePresenter _mainPagePresneter;
		private IRoomPresenter _roomPresneter;
		private ITopMenuView _topMenuView;
		private BackendPlayerData _backendPlayerData;
		
		public MetagamePresenter(
			IHTTPPresenter hTTPPresenter,
			IWarningPresenter warningPresenter,
			IMainPagePresenter mainPagePresneter,
			IRoomPresenter roomPresneter,
			ITopMenuView topMenuView,
			BackendPlayerData backendPlayerData)
		{
			_hTTPPresenter = hTTPPresenter;
			_warningPresenter = warningPresenter;
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
			yield return WebUtility.RunAndHandleInternetError(_hTTPPresenter.GetSelfPlayerData(), _warningPresenter);
			
			_topMenuView.Enter(_backendPlayerData);
			
			var nextStatus = new MetagameStatus(MetagameStatusType.MainPage);
			while (nextStatus.Type != MetagameStatusType.EnterGame)
			{
				var monad = 
					(nextStatus.Type == MetagameStatusType.MainPage) 
						? new BlockMonad<MetagameStatus>(r => _mainPagePresneter.Run(r))
						: new BlockMonad<MetagameStatus>(r => _roomPresneter.Run(nextStatus.ToRoomId, r));
				yield return monad.Do();
				if (monad.Error != null)
				{
					Debug.LogError(monad.Error);
					break;
				}
				
				nextStatus = monad.Result;
			}
		}
	}
}
