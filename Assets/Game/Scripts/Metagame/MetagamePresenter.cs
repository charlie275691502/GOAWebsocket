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
		private IRoomWebSocketPresenter _webSocketPresenter;
		
		private CommandExecutor _commandExecutor = new CommandExecutor();
		private CommandExecutor _webSocketCommandExecutor = new CommandExecutor();
		
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
		
		public IEnumerator Run()
		{
			_commandExecutor.Add(_Run());
			_commandExecutor.Add(_webSocketCommandExecutor.Start());
			yield return _commandExecutor.Start();
		}
		
		private IEnumerator _Run()
		{
			yield return _hTTPPresenter.GetSelfPlayerData().RunAndHandleInternetError(_warningPresenter);
			
			_topMenuView.Enter(_backendPlayerData);
			
			var nextStatus = new MetagameStatus(MetagameStatusType.MainPage);
			while (nextStatus.Type != MetagameStatusType.EnterGame)
			{
				var monad = 
					(nextStatus.Type == MetagameStatusType.MainPage) 
						? new BlockMonad<MetagameStatus>(r => _mainPagePresneter.Run(r, _OnJoinRoom))
						: new BlockMonad<MetagameStatus>(r => _roomPresneter.Run(nextStatus.ToRoomId, _OnSendMessage, _OnLeaveRoom, r));
				yield return monad.Do();
				if (monad.Error != null)
				{
					Debug.LogError(monad.Error);
					break;
				}
				
				nextStatus = monad.Result;
			}
			
			_topMenuView.Leave();
			_Stop();
		}
		
		private void _Stop()
		{
			_commandExecutor.Stop();
			_commandExecutor.Clear();
			_webSocketPresenter.Stop();
			_webSocketCommandExecutor.Clear();
		}
		
		private void _OnJoinRoom(int roomId)
		{
			_webSocketCommandExecutor.TryAdd(_JoinRoom(roomId));
		}
		
		private void _OnLeaveRoom(int roomId)
		{
			_webSocketPresenter.LeaveRoom(roomId);
			_webSocketPresenter.Stop();
		}
		
		private void _OnSendMessage(string message)
		{
			_webSocketPresenter.SendMessage(message);
		}
		
		private IEnumerator _JoinRoom(int roomId)
		{
			_webSocketPresenter.RegisterOnReceiveAppendMessage(_OnReceiveAppendMessage);
			_webSocketPresenter.RegisterOnReceiveUpdateRoom(_OnReceiveUpdateRoom);
			var webSocketMonad = _webSocketPresenter.Run(roomId);
			yield return webSocketMonad.RunAndHandleInternetError(_warningPresenter);
			if(webSocketMonad.Error != null)
			{
				yield break;
			}
			
			_webSocketPresenter.JoinRoom(roomId);
			_mainPagePresneter.SwitchToRoom(roomId);
		}
		
		private void _OnReceiveAppendMessage(MessageResult result)
		{
			_roomPresneter.AppendMessage(result);
		}
		
		private void _OnReceiveUpdateRoom(RoomResult result)
		{
			
		}
	}
}
