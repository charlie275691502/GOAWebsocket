using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Warning;
using Cysharp.Threading.Tasks;
using Optional;
using Rayark.Mast;
using UnityEngine;
using Web;

namespace Metagame
{
	public record CreateRoomReturn(
		string RoomName,
		GameType GameType,
		int PlayerPlot);
		
	public interface ICreateRoomPresenter
	{
		UniTask<CreateRoomReturn> Run();
	}
	
	public class CreateRoomPresenter : ICreateRoomPresenter
	{
		
		private IWarningPresenter _warningPresenter;
		private ICreateRoomView _view;
		
		private CommandExecutor _commandExecutor = new CommandExecutor();
		private Option<CreateRoomReturn> _result;
		
		public CreateRoomPresenter(IWarningPresenter warningPresenter, ICreateRoomView view)
		{
			_warningPresenter = warningPresenter;
			_view = view;
		}
		
		public IEnumerator Run(IReturn<CreateRoomReturn> ret)
		{
			_view.Enter(_OnConfirm, _OnCancel);
			
			_commandExecutor.Clear();
			yield return _commandExecutor.Start();
			
			_result.Match(
				result => ret.Accept(result),
				() => ret.Fail(new Exception("User Cancel"))
			);
		}
		
		private void _Stop()
		{
			_view.Leave();
			_commandExecutor.Stop();
		}
		
		private void _OnConfirm(string roomName, GameType gameType, int playerPlot)
		{
			_commandExecutor.TryAdd(_CreateRoom(roomName, gameType, playerPlot));
		}
		
		private void _OnCancel()
		{
			_result = Option.None<CreateRoomReturn>();
			_Stop();
		}
		
		public IEnumerator _CreateRoom(string roomName, GameType gameType, int playerPlot)
		{
			if(string.IsNullOrEmpty(roomName))
			{
				yield return _warningPresenter.Run("Create Room Error", "Room name cannot be empty");
				yield break;
			}
			
			_result = new CreateRoomReturn(
				RoomName: roomName,
				GameType: gameType,
				PlayerPlot: playerPlot
			).Some();
			
			_Stop();
		}
	}
}