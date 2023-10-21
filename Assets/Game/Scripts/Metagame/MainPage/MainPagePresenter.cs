using System;
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
		IEnumerator Run(IReturn<MetagameStatus> ret, Action<int> onJoinRoom);
		void SwitchToRoom(int roomId);
	}
	
	public class MainPagePresenter : IMainPagePresenter
	{
		private IHTTPPresenter _hTTPPresenter;
		private IWarningPresenter _warningPresenter;
		private ICreateRoomPresenter _createRoomPresenter;
		private IMainPageView _view;
		
		private CommandExecutor _commandExecutor = new CommandExecutor();
		private MetagameStatus _result;
		private Action<int> _onJoinRoom;
		
		public MainPagePresenter(IHTTPPresenter hTTPPresenter, IWarningPresenter warningPresenter, ICreateRoomPresenter createRoomPresenter, IMainPageView view)
		{
			_hTTPPresenter = hTTPPresenter;
			_warningPresenter = warningPresenter;
			_createRoomPresenter = createRoomPresenter;
			_view = view;
		}
		
		public IEnumerator Run(IReturn<MetagameStatus> ret, Action<int> onJoinRoom)
		{
			_Register(onJoinRoom);
			var monad = _hTTPPresenter.GetRoomList();
			yield return monad.RunAndHandleInternetError(_warningPresenter);
			if(monad.Error != null)
			{
				yield break;
			}
			
			var roomViewDatas = _GetRoomViewDatas(monad.Result);
			_view.Enter(roomViewDatas, _OnJoinRoom, _OnCreateRoom);
			
			_commandExecutor.Clear();
			yield return _commandExecutor.Start();
			ret.Accept(_result);
		}
		
		private void _Stop()
		{
			_Unregister();
			_view.Leave();
			_commandExecutor.Stop();
		}
		
		private void _Register(Action<int> onJoinRoom)
		{
			_onJoinRoom = onJoinRoom;
		}
		
		private void _Unregister()
		{
			_onJoinRoom = null;
		}
		
		private void _OnJoinRoom(int roomId)
		{
			_commandExecutor.TryAdd(_JoinRoom(roomId));
		}
		
		private IEnumerator _JoinRoom(int roomId)
		{
			_onJoinRoom?.Invoke(roomId);
			yield break;
		}
		
		private void _OnCreateRoom()
		{
			_commandExecutor.TryAdd(_CreateRoom());
		}
		
		private IEnumerator _CreateRoom()
		{
			var createRoomMonad = new BlockMonad<string>(_createRoomPresenter.Run);
			yield return createRoomMonad.Do();
			if (createRoomMonad.Error != null)
			{
				yield break;
			}
			
			var monad = _hTTPPresenter.CreateRoom(createRoomMonad.Result);
			yield return monad.RunAndHandleInternetError(_warningPresenter);
			if(monad.Error != null)
			{
				yield break;
			}
			
			yield return _JoinRoom(monad.Result.Id);
		}
		
		private List<RoomViewData> _GetRoomViewDatas(RoomListResult result)
		{
			return result.Select(roomResult => new RoomViewData()
				{
					Id = roomResult.Id,
					RoomName = roomResult.RoomName,
					GameType = GameTypeUtility.GetGameType(roomResult.GameSetting.GameType),
					Players = roomResult.Players.Select(playerDataResult => new PlayerData(playerDataResult)).ToList(),
				}).ToList();
		}
		
		public void SwitchToRoom(int roomId)
		{
			_result = new MetagameStatus(MetagameStatusType.Room, roomId);
			_Stop();
		}
	}
}