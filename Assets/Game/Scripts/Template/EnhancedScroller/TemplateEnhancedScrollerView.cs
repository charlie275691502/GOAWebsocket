using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Template
{
	public class TemplateEnhancedScrollerView : MonoBehaviour
	{
		[Header("Scroller")]
		[SerializeField]
		private EnhancedScrollerProxy _scroller;
		[SerializeField]
		private GameObject _prefab;
		[SerializeField]
		private string _scrollingAudioKey;
		[SerializeField]
		private float _lookAheadBefore;
		[SerializeField]
		private float _lookAheadAfter;
		
		private SimpleEnhancedScrollerController _scrollerController;
		private EnhancedScrollerDataModel<TemplateEnhancedScrollerElementView, EnhancedScrollerElementViewData<TemplateViewData>> _dataModel;
		
		[Zenject.Inject]
		public void Zenject(DiContainer container)
		{
			_dataModel = new EnhancedScrollerDataModel<TemplateEnhancedScrollerElementView, EnhancedScrollerElementViewData<TemplateViewData>>(
				_prefab,
				_scroller.scrollDirection);
			_dataModel.OnInstantiateCell += _OnInstantiateCell;
				
			_scrollerController = new SimpleEnhancedScrollerController(
				_scroller,
				_dataModel,
				_scrollingAudioKey,
				_lookAheadBefore,
				_lookAheadAfter,
				container);
		}
		
		private void _DisplayEnhancedScroller(List<TemplateViewData> viewDatas)
		{
			_dataModel.UpdateViewDatas(EnhancedScrollerUtility.GetViewDataList(viewDatas));
			_scrollerController.Display();
		}
		
		private void _Leave()
		{
			_scrollerController.Clear();
		}
		
		private void _OnInstantiateCell(TemplateEnhancedScrollerElementView view, EnhancedScrollerElementViewData<TemplateViewData> viewData, int dataIndex)
		{
			view.OnClick += () => _OnClick(viewData.Data);
		}
		
		private void _OnClick(TemplateViewData viewData)
		{
			
		}
	}
}
