using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
	public interface ILoadingView
	{
		void Enter();
		void Leave();
	}
	
	public class LoadingView : MonoBehaviour, ILoadingView
	{
		[SerializeField]
		private GameObject _panel;
		
		public void Enter()
		{
			_panel.SetActive(true);
		}
		
		public void Leave()
		{
			_panel.SetActive(false);
		}
	}
}