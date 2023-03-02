using System.Collections;
using System.Collections.Generic;
using Rayark.Mast;
using UnityEngine;

namespace Common
{
	public class CommandExecutor : Executor
	{
		public bool IsRunning { get{return !_isStop; }}
		private bool _isStop = true;
		public IEnumerator Start()
		{
			_isStop = false;
			while(!_isStop)
			{
				Resume(Time.deltaTime);
				yield return null;
			}
		}
		
		public void Stop()
		{
			_isStop = true;
		}
	}
}