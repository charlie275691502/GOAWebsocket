using System.Collections;
using System.Collections.Generic;
using Common;
using Rayark.Mast;
using UnityEngine;
using Web;
using System.Linq;
using System;

namespace Metagame
{
	public interface IRoomPresenter
	{
		IEnumerator Run(int roomId, Action<string> onSendMessage, Action<int> onLeaveRoom, IReturn<MetagameStatus> ret);
		void AppendMessage(MessageResult result);
	}
	
	public class RoomPresenter : IRoomPresenter
	{
		private IHTTPPresenter _hTTPPresenter;
		private IWarningPresenter _warningPresenter;
		private IRoomView _view;
		
		private Action<string> _onSendMessage;
		private Action<int> _onLeaveRoom;
		private CommandExecutor _commandExecutor = new CommandExecutor();
		private MetagameStatus _result;
		private int _roomId;
		
		public RoomPresenter(IHTTPPresenter hTTPPresenter, IWarningPresenter warningPresenter, IRoomView view)
		{
			_hTTPPresenter = hTTPPresenter;
			_warningPresenter = warningPresenter;
			_view = view;
		}
		
		public IEnumerator Run(int roomId, Action<string> onSendMessage, Action<int> onLeaveRoom, IReturn<MetagameStatus> ret)
		{
			_Register(onSendMessage, onLeaveRoom);
			_roomId = roomId;
			var httpMonad = _hTTPPresenter.GetRoomWithMessages(roomId);
			
			yield return httpMonad.RunAndHandleInternetError(_warningPresenter);
			if(httpMonad.Error != null)
			{
				yield break;
			}
			
			var roomWithMessagesViewData = _GetRoomWithMessagesViewData(httpMonad.Result);
			_view.Enter(roomWithMessagesViewData, _OnLeaveRoom, _OnSendMessage);
			
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
		
		private void _Register(Action<string> onSendMessage, Action<int> onLeaveRoom)
		{
			_onSendMessage = onSendMessage;
			_onLeaveRoom = onLeaveRoom;
		}
		
		private void _Unregister()
		{
			_onSendMessage = null;
			_onLeaveRoom = null;
		}
		
		private void _OnLeaveRoom()
		{
			_onLeaveRoom?.Invoke(_roomId);
			_result = new MetagameStatus(MetagameStatusType.MainPage);
			_Stop();
		}
		
		private void _OnSendMessage(string message)
		{
			_onSendMessage?.Invoke(message);
		}
		
		private RoomWithMessagesViewData _GetRoomWithMessagesViewData(RoomWithMessagesResult result)
		{
			return new RoomWithMessagesViewData()
			{
					Id = result.Id,
					RoomName = result.RoomName,
					Players = result.Players.Select(playerDataResult => new PlayerData(playerDataResult)).ToList(),
					Messages = result.Messages.Select(message => new MessageViewData()
					{
						Id = message.Id,
						Content = message.Content,
						NickName = message.Player.NickName,
					}).ToList(),
			};
		}
		
		public void AppendMessage(MessageResult result)
		{
			_view.AppendMessage(new MessageViewData()
			{
				Id = result.Id,
				Content = result.Content,
				NickName = result.Player.NickName,
			});
		}
	}
}