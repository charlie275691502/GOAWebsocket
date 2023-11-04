using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Warning
{
	public interface IWarningPresenter
	{
		UniTask Run(string title, string content, Action onConfirm = null);
	}

	public abstract record WarningState
	{
		public record Open() : WarningState;
		public record Idle() : WarningState;
		public record Confirm() : WarningState;
		public record Close() : WarningState;
	}

	public record WarningProperty(
		WarningState State,
		string Title,
		string Content);
	
	public class WarningPresenter : IWarningPresenter
	{
		private IWarningView _view;

		private WarningProperty _prop;

		public WarningPresenter(IWarningView view)
		{
			_view = view;

			_view.RegisterCallback(
				() =>
					_ChangeStateIfIdle(new WarningState.Confirm()));
		}

		async UniTask IWarningPresenter.Run(string title, string content, Action onConfirm = null)
		{
			_prop = new WarningProperty(
				new WarningState.Open(),
				title,
				content);

			while (_prop.State is not WarningState.Close)
			{
				_view.Render(_prop);
				switch (_prop.State)
				{
					case WarningState.Open:
						_prop = _prop with { State = new WarningState.Idle() };
						break;

					case WarningState.Idle:
						break;

					case WarningState.Confirm:
						onConfirm?.Invoke();
						_prop = _prop with { State = new WarningState.Close() };
						break;

					case WarningState.Close:
						_prop = _prop with { State = new WarningState.Close() };
						break;

					default:
						break;
				}
				await UniTask.Yield();
			}
			
			_view.Render(_prop);
		}

		private void _ChangeStateIfIdle(WarningState targetState, Action onChangeStateSuccess = null)
		{
			if (_prop.State is not WarningState.Idle)
				return;

			onChangeStateSuccess?.Invoke();
			_prop = _prop with { State = targetState };
		}
	}
}