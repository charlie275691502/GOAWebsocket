using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Rayark.Mast;
using UnityEngine;
using Web;

namespace Metagame
{
	public interface IMainPagePresenter
	{
		IEnumerator Run(IReturn<MetagameStatus> ret);
	}
	
	public class MainPagePresenter : IMainPagePresenter
	{
		private IHTTPPresenter _hTTPPresenter;
		private IWarningPresenter _warningPresenter;
		private IMainPageView _view;
		
		private CommandExecutor _commandExecutor = new CommandExecutor();
		private MetagameStatus _result;
		
		public MainPagePresenter(IHTTPPresenter hTTPPresenter, IWarningPresenter warningPresenter, IMainPageView view)
		{
			_hTTPPresenter = hTTPPresenter;
			_warningPresenter = warningPresenter;
			_view = view;
		}
		
		public IEnumerator Run(IReturn<MetagameStatus> ret)
		{
			var monad = _hTTPPresenter.GetRoomList();
			yield return WebUtility.RunAndHandleInternetError(monad, _warningPresenter);
			if(monad.Error != null)
			{
				yield break;
			}
			
			var roomViewDatas = _GetRoomViewDatas(monad.Result);
			_view.Enter(roomViewDatas, _SwitchToRoom);
			
			_commandExecutor.Clear();
			yield return _commandExecutor.Start();
			ret.Accept(_result);
		}
		
		private void _Stop()
		{
			_view.Leave();
			_commandExecutor.Stop();
		}
		
		private void _SwitchToRoom(int roomId)
		{
			_result = new MetagameStatus(MetagameStatusType.Room, roomId);
			_Stop();
		}
		
		private List<RoomViewData> _GetRoomViewDatas(RoomListResult result)
		{
			return result.Select(roomResult => new RoomViewData()
				{
					Id = roomResult.Id,
					RoomName = roomResult.RoomName,
					Players = roomResult.Players.Select(playerDataResult => new PlayerData(playerDataResult)).ToList(),
				}).ToList();
		}
	}
}