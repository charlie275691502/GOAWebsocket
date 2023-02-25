using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	public interface IWarningPresenter
	{
		IEnumerator Run(string title, string content, Action onConfirm = null);
	}
	
	public class WarningPresenter : IWarningPresenter
	{
		private readonly IWarningView _warningView;
		
		private CommandExecutor _commandExecutor = new CommandExecutor();
		private Action _onConfirm;
		
		public WarningPresenter(IWarningView warningView)
		{
			_warningView = warningView;
		}
		
		public IEnumerator Run(string title, string content, Action onConfirm = null)
		{
			_onConfirm = onConfirm;
			_warningView.Enter(title, content, _OnConfirm);
			_commandExecutor.Clear();
			yield return _commandExecutor.Start();
		}
		
		private void _Stop()
		{
			_warningView.Leave();
			_commandExecutor.Stop();
		}
		
		private void _OnConfirm()
		{
			_onConfirm?.Invoke();
			_Stop();
		}
	}
}