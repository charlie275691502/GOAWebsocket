using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.Linq;
using Common;
using Web;
using System;

namespace Common
{
	public class ActionQueue
	{
		private List<Action> _actions;

		public ActionQueue()
        {
			_actions = new List<Action>();
        }

		public void Add(Action action)
        {
			_actions.Add(action);
		}

		public void RunAll()
        {
			_actions.ForEach(action => action?.Invoke());
			_actions.Clear();
		}
	}
}