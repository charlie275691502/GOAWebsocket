using System.Collections;
using System.Collections.Generic;
using Common;
using Rayark.Mast;
using UnityEngine;
using Web;

namespace Template
{
	public enum TemplateTabResult
	{
		
	}
	
	public interface ITemplatePresenter
	{
		IEnumerator Run(IReturn<TemplateTabResult> ret);
	}
	
	public class TemplatePresenter : ITemplatePresenter
	{
		private IHTTPPresenter _hTTPPresenter;
		private IWarningPresenter _warningPresenter;
		private ITemplateView _view;
		private TemplateTabResult _result;
		
		private CommandExecutor _commandExecutor = new CommandExecutor();
		
		public TemplatePresenter(IHTTPPresenter hTTPPresenter, IWarningPresenter warningPresenter, ITemplateView view)
		{
			_hTTPPresenter = hTTPPresenter;
			_warningPresenter = warningPresenter;
			_view = view;
		}
		
		public IEnumerator Run(IReturn<TemplateTabResult> ret)
		{
			_view.Enter();
			_commandExecutor.Clear();
			yield return _commandExecutor.Start();
			ret.Accept(_result);
		}
		
		private void _Stop()
		{
			_view.Leave();
			_commandExecutor.Stop();
		}
	}
}