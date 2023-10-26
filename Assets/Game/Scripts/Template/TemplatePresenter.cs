using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Common.Warning;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Web;

namespace Template
{
	public enum TemplateReturnType
	{
		Confirm,
		Close,
	}
	
	public interface ITemplatePresenter
	{
		UniTask<TemplateReturnType> Run();
	}
	
	public abstract record TemplateState
	{
		public record Open() : TemplateState;
		public record Idle() : TemplateState;
		public record Confirm() : TemplateState;
		public record Close() : TemplateState;
	}

	public record TemplateProperty(TemplateState State, TemplateReturnType ReturnType);

	public class TemplatePresenter : ITemplatePresenter
	{
		private IHTTPPresenter _hTTPPresenter;
		private IWarningPresenter _warningPresenter;
		private ITemplateView _view;

		private TemplateProperty _prop;

		public TemplatePresenter(IHTTPPresenter hTTPPresenter, IWarningPresenter warningPresenter, ITemplateView view)
		{
			_hTTPPresenter = hTTPPresenter;
			_warningPresenter = warningPresenter;
			_view = view;

			_view.RegisterCallback(
				() =>
					_ChangeStateIfIdle(new TemplateState.Confirm()));
		}

		public async UniTask<TemplateReturnType> Run()
		{
			_prop = new TemplateProperty(new TemplateState.Open(), TemplateReturnType.Close);

			while (_prop.State is not TemplateState.Close)
			{
				_view.Render(_prop);
				switch (_prop.State)
				{
					case TemplateState.Open:
						_prop = _prop with { State = new TemplateState.Idle() };
						break;

					case TemplateState.Idle:
						break;

					case TemplateState.Confirm info:
						_prop = _prop with
						{
							State = new TemplateState.Close(),
							ReturnType = TemplateReturnType.Confirm
						};
						break;

					case TemplateState.Close:
						_prop = _prop with { State = new TemplateState.Close() };
						break;

					default:
						break;
				}
				await UniTask.Yield();
			}

			return _prop.ReturnType;
		}

		private void _ChangeStateIfIdle(TemplateState targetState, Action onChangeStateSuccess = null)
		{
			if (_prop.State is not TemplateState.Idle)
				return;

			onChangeStateSuccess?.Invoke();
			_prop = _prop with { State = targetState };
		}
	}
}