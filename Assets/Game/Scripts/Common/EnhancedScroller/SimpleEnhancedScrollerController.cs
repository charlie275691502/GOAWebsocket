using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnhancedUI.EnhancedScroller
{
	public interface ISimpleEnhancedScrollerElement<T>
	{
		void Enter(T viewData);
		void Leave();
	}
	
	/// <summary>
	/// T1 is view
	/// T2 is viewData
	/// </summary>
	public class SimpleEnhancedScrollerController<T1, T2>: IEnhancedScrollerDelegate where T1 : EnhancedScrollerCellView, ISimpleEnhancedScrollerElement<T2>
	{
		private EnhancedScroller _scroller;
		private T1 _prefabView;
		private float _cellViewSize;
		private List<T2> _viewDatas = new List<T2>();
		private GameObject _headSliderHint;
		private GameObject _tailliderHint;
		public Action<T1, T2, int> OnInstantiateCell;
		public Action<T1> OnReturnCell;
		
		public void RegisterHeadSliderHint(GameObject headSliderHint)
		{
			_headSliderHint = headSliderHint;
		}
		
		public void RegisterTailSliderHint(GameObject tailliderHint)
		{
			_tailliderHint = tailliderHint;
		}
		
		public void Init(EnhancedScroller scroller, GameObject prefab)
		{
			_scroller = scroller;
			_prefabView = prefab.GetComponent<T1>();
			_cellViewSize = (_scroller.scrollDirection == EnhancedScroller.ScrollDirectionEnum.Horizontal) 
				? prefab.GetComponent<RectTransform>().rect.width 
				: prefab.GetComponent<RectTransform>().rect.height;
			_scroller.cellViewWillRecycle += _OnReturnCell;
			_scroller.Delegate = this;
			_scroller.Init();
		}
		
		public void Leave()
		{
			_viewDatas.Clear();
			_scroller.ReloadData();
		}
		
		public void Display(List<T2> viewDatas, float scrollPositionFactor = 0)
		{
			_viewDatas = viewDatas;
			_scroller.ReloadData(scrollPositionFactor);
		}
		
		public int GetNumberOfCells(EnhancedScroller scroller)
		{
			return _viewDatas.Count;
		}
		
		public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
		{
			return _cellViewSize;
		}

		public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
		{
			var view = scroller.GetCellView(_prefabView) as T1;
			OnInstantiateCell?.Invoke(view, _viewDatas[dataIndex], dataIndex);
			view.Enter(_viewDatas[dataIndex]);
			
			if(dataIndex == 0)
			{
				_headSliderHint?.SetActive(false);
			} else if (dataIndex == _viewDatas.Count - 1)
			{
				_tailliderHint?.SetActive(false);
			}
			
			return view;
		}
		
		private void _OnReturnCell(EnhancedScrollerCellView cellView)
		{
			var view = cellView as T1;
			OnReturnCell?.Invoke(view);
			view.Leave();
			
			if(cellView.dataIndex == 0)
			{
				_headSliderHint?.SetActive(true);
			} else if (cellView.dataIndex == _viewDatas.Count - 1)
			{
				_tailliderHint?.SetActive(true);
			}
		}
	}
}