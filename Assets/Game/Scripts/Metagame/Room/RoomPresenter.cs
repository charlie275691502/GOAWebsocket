using System.Collections;
using System.Collections.Generic;
using Common;
using Rayark.Mast;
using UnityEngine;
using Web;
using System.Linq;

namespace Metagame
{
	public interface IRoomPresenter
	{
		IEnumerator Run(int roomId, IReturn<MetagameStatus> ret);
	}
	
	public class RoomPresenter : IRoomPresenter
	{
		private IHTTPPresenter _hTTPPresenter;
		private IRoomWebSocketPresenter _webSocketPresenter;
		private IWarningPresenter _warningPresenter;
		private IRoomView _view;
		
		private CommandExecutor _commandExecutor = new CommandExecutor();
		private MetagameStatus _result;
		
		public RoomPresenter(IHTTPPresenter hTTPPresenter, IRoomWebSocketPresenter webSocketPresenter, IWarningPresenter warningPresenter, IRoomView view)
		{
			_hTTPPresenter = hTTPPresenter;
			_webSocketPresenter = webSocketPresenter;
			_warningPresenter = warningPresenter;
			_view = view;
		}
		
		public IEnumerator Run(int roomId, IReturn<MetagameStatus> ret)
		{
			var httpMonad = _hTTPPresenter.GetRoomWithMessages(roomId);
			var webSocketMonad = _webSocketPresenter.Run(roomId);
			
			yield return Monad.WhenAll(httpMonad, webSocketMonad).RunAndHandleInternetError(_warningPresenter);
			if(httpMonad.Error != null || webSocketMonad.Error != null)
			{
				yield break;
			}
			
			// yield return httpMonad.RunAndHandleInternetError(_warningPresenter);
			// yield return webSocketMonad.RunAndHandleInternetError(_warningPresenter);
			
			var roomWithMessagesViewData = _GetRoomWithMessagesViewData(httpMonad.Result);
			_view.Enter(roomWithMessagesViewData, _SwitchToMainPage, _OnSendMessage);
			
			_commandExecutor.Clear();
			yield return _commandExecutor.Start();
			ret.Accept(_result);
		}
		
		private void _Stop()
		{
			_view.Leave();
			_webSocketPresenter.Stop();
			_commandExecutor.Stop();
		}
		
		private void _SwitchToMainPage()
		{
			_result = new MetagameStatus(MetagameStatusType.MainPage);
			_Stop();
		}
		
		private void _OnSendMessage(string message)
		{
			_webSocketPresenter.Message(message);
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
	}
}