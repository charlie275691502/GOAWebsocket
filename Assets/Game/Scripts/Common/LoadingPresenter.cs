using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	public interface ILoadingPresenter
	{
		void Run();
		void Stop();
	}
	
	public class LoadingPresenter : ILoadingPresenter
	{
		private readonly ILoadingView _loadingView;
		
		public LoadingPresenter(ILoadingView loadingView)
		{
			_loadingView = loadingView;
		}
		
		public void Run()
		{
			_loadingView.Enter();
		}
		
		public void Stop()
		{
			_loadingView.Leave();
		}
	}
}