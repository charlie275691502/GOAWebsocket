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
	public interface ICreateRoomPresenter
	{
		IEnumerator Run(IReturn<string> ret);
	}
	
	public class CreateRoomPresenter : ICreateRoomPresenter
	{
		private IWarningPresenter _warningPresenter;
		private ICreateRoomView _view;
		
		private CommandExecutor _commandExecutor = new CommandExecutor();
		private string _result;
		
		public CreateRoomPresenter(IWarningPresenter warningPresenter, ICreateRoomView view)
		{
			_warningPresenter = warningPresenter;
			_view = view;
		}
		
		public IEnumerator Run(IReturn<string> ret)
		{
			_view.Enter(_OnConfirm, _OnCancel);
			
			_commandExecutor.Clear();
			yield return _commandExecutor.Start();
			
			if(string.IsNullOrEmpty(_result))
			{
				ret.Fail(new Exception("User Cancel"));
			} else 
			{
				ret.Accept(_result);
			}
		}
		
		private void _Stop()
		{
			_view.Leave();
			_commandExecutor.Stop();
		}
		
		private void _OnConfirm(string roomName)
		{
			_commandExecutor.TryAdd(_CreateRoom(roomName));
		}
		
		private void _OnCancel()
		{
			_result = string.Empty;
			_Stop();
		}
		
		public IEnumerator _CreateRoom(string roomName)
		{
			if(string.IsNullOrEmpty(roomName))
			{
				yield return _warningPresenter.Run("Create Room Error", "Room name cannot be empty");
				yield break;
			}
			
			_result = roomName;
			_Stop();
		}
	}
}