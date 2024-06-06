using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.GOA
{
	public class GOACardDetaialViewData
	{
		
	}
	
	public class GOACardDetaialView : MonoBehaviour
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private Image _image;
		[SerializeField]
		private Button _useButton;

		public void RegisterCallback(Action onClickUseButton)
		{
			_useButton.onClick.AddListener(() => onClickUseButton?.Invoke());
		}

		public void Open(GOACardDetaialViewData viewData)
		{
			_panel.SetActive(true);
		}

		public void Close()
		{
			_panel.SetActive(false);
		}
	}
}
