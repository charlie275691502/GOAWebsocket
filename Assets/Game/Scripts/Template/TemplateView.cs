using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace Template
{
	public interface ITemplateView
	{
		void Enter();
		void Leave();
	}
	
	public class TemplateView : MonoBehaviour, ITemplateView
	{
		[SerializeField]
		private GameObject _panel;
		
		public void Enter()
		{
			_Enter();
			_Register();
			_panel.SetActive(true);
		}
		
		public void Leave()
		{
			_Unregister();
			_panel.SetActive(false);
			_Leave();
		}
		
		private void _Enter()
		{
		}
		
		private void _Leave()
		{
		}
		
		private void _Register()
		{
		}
		
		private void _Unregister()
		{
		}
	}
}
