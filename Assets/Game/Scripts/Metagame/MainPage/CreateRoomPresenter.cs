using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Warning;
using Cysharp.Threading.Tasks;
using Optional;
using UnityEngine;
using Web;

namespace Metagame.MainPage.CreateRoom
{
	public record CreateRoomReturnType()
	{
		public record Confirm(string RoomName, GameType GameType, int PlayerPlot) : CreateRoomReturnType;
		public record Cancel() : CreateRoomReturnType;
	}

	public abstract record CreateRoomState
	{
		public record Open() : CreateRoomState;
		public record Idle() : CreateRoomState;
		public record Confirm(string RoomName, GameType GameType, int PlayerPlot) : CreateRoomState;
		public record Cancel() : CreateRoomState;
		public record Close() : CreateRoomState;
	}

	public record CreateRoomProperty(CreateRoomState State);
	public record CreateRoomReturn(CreateRoomReturnType Type);

	public interface ICreateRoomPresenter
	{
		UniTask<CreateRoomReturn> Run();
	}
	
	public class CreateRoomPresenter : ICreateRoomPresenter
	{
		private IWarningPresenter _warningPresenter;
		private ICreateRoomView _view;
		
		private CreateRoomProperty _prop;

		public CreateRoomPresenter(IWarningPresenter warningPresenter, ICreateRoomView view)
		{
			_warningPresenter = warningPresenter;
			_view = view;
		}
		
		async UniTask<CreateRoomReturn> ICreateRoomPresenter.Run()
		{
			_prop = new CreateRoomProperty(new CreateRoomState.Open());
			var ret = new CreateRoomReturn(new CreateRoomReturnType.Cancel());

			while (_prop.State is not CreateRoomState.Close)
			{
				_view.Render(_prop);
				switch (_prop.State)
				{
					case CreateRoomState.Open:
						_prop = _prop with { State = new CreateRoomState.Idle() };
						break;

					case CreateRoomState.Idle:
						break;

					case CreateRoomState.Confirm info:
						var isValid = await _ValidateRoom(info.RoomName, info.GameType, info.PlayerPlot);
						if (isValid)
                        {
							ret = ret with { Type = new CreateRoomReturnType.Confirm(info.RoomName, info.GameType, info.PlayerPlot) };
							_prop = _prop with { State = new CreateRoomState.Close() };
						} else
                        {
							_prop = _prop with { State = new CreateRoomState.Idle() };
						}
						break;

					case CreateRoomState.Cancel:
						ret = ret with { Type = new CreateRoomReturnType.Cancel() };
						_prop = _prop with { State = new CreateRoomState.Close() };
						break;

					case CreateRoomState.Close:
						break;

					default:
						break;
				}
				await UniTask.Yield();
			}

			return ret;
		}

		private void _ChangeStateIfIdle(CreateRoomState targetState, Action onChangeStateSuccess = null)
		{
			if (_prop.State is not CreateRoomState.Idle)
				return;

			onChangeStateSuccess?.Invoke();
			_prop = _prop with { State = targetState };
		}
		
		private async UniTask<bool> _ValidateRoom(string roomName, GameType gameType, int playerPlot)
		{
			if(string.IsNullOrEmpty(roomName))
			{
				await _warningPresenter.Run("Create Room Error", "Room name cannot be empty");
				return false;
			}

			return true;
		}
	}
}