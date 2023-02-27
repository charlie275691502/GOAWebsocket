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
		private IWarningPresenter _warningPresenter;
		private IRoomView _view;
		
		private CommandExecutor _commandExecutor = new CommandExecutor();
		private MetagameStatus _result;
		
		public RoomPresenter(IHTTPPresenter hTTPPresenter, IWarningPresenter warningPresenter, IRoomView view)
		{
			_hTTPPresenter = hTTPPresenter;
			_warningPresenter = warningPresenter;
			_view = view;
		}
		
		public IEnumerator Run(int roomId, IReturn<MetagameStatus> ret)
		{
			var monad = _hTTPPresenter.GetRoomWithMessages(roomId);
			yield return WebUtility.RunAndHandleInternetError(monad, _warningPresenter);
			if(monad.Error != null)
			{
				yield break;
			}
			
			var roomWithMessagesViewData = _GetRoomWithMessagesViewData(monad.Result);
			_view.Enter(roomWithMessagesViewData);
			
			_commandExecutor.Clear();
			yield return _commandExecutor.Start();
			ret.Accept(_result);
		}
		
		private void _Stop()
		{
			_view.Leave();
			_commandExecutor.Stop();
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