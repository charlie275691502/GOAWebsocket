using System.Collections;
using System.Collections.Generic;
using Common;
using Rayark.Mast;
using UnityEngine;
using Web;

namespace Metagame
{
	public interface IRoomPresenter
	{
		IEnumerator Run(IReturn<MetagameTabResult> ret);
	}
	
	public class RoomPresenter : IRoomPresenter
	{
		private IHTTPPresenter _hTTPPresenter;
		private IWarningPresenter _warningPresenter;
		private IRoomView _view;
		
		private CommandExecutor _commandExecutor = new CommandExecutor();
		private MetagameTabResult _result;
		
		public RoomPresenter(IHTTPPresenter hTTPPresenter, IWarningPresenter warningPresenter, IRoomView view)
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