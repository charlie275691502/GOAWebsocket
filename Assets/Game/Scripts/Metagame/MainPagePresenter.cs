using System.Collections;
using System.Collections.Generic;
using Common;
using Rayark.Mast;
using UnityEngine;
using Web;

namespace Metagame
{
	public interface IMainPagePresenter
	{
		IEnumerator Run(IReturn<MetagameTabResult> ret);
	}
	
	public class MainPagePresenter : IMainPagePresenter
	{
		private IHTTPPresenter _hTTPPresenter;
		private IWarningPresenter _warningPresenter;
		private IMainPageView _view;
		
		private CommandExecutor _commandExecutor = new CommandExecutor();
		private MetagameTabResult _result;
		
		public MainPagePresenter(IHTTPPresenter hTTPPresenter, IWarningPresenter warningPresenter, IMainPageView view)
		{
			_hTTPPresenter = hTTPPresenter;
			_warningPresenter = warningPresenter;
			_view = view;
		}
		
		public IEnumerator Run(IReturn<MetagameTabResult> ret)
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