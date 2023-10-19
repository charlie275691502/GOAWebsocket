using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using EnhancedUI.EnhancedScroller.Internal;

namespace EnhancedUI.EnhancedScroller
{
    public class SimpleEnhancedScrollerController: IEnhancedScrollerDelegate
    {
        protected EnhancedScrollerProxy _scroller;
        protected IEnhancedScrollerDataModel _dataModel;
        protected string _scrollingAudioKey;
        protected float _lookAheadBefore;
        protected float _lookAheadAfter;
        protected GameObject _headSliderHint;
        protected GameObject _tailSliderHint;
        protected DiContainer _container;
        
        protected bool _isFinishDisplay = false;
        protected int _prevStartDataIndex;
        protected int _prevEndDataIndex;
        
        public bool IsFinishDisplay { get { return _isFinishDisplay; }}
        
        public SimpleEnhancedScrollerController(
            EnhancedScrollerProxy scroller,
            IEnhancedScrollerDataModel dataModel,
            string scrollingAudioKey,
            float lookAheadBefore,
            float lookAheadAfter,
            DiContainer container)
        {
            _scroller = scroller;
            _dataModel = dataModel;
            _scrollingAudioKey = scrollingAudioKey;
            _scroller.scrollerScrolled += _OnScrollerScrolled;
            _scroller.cellViewWillRecycle += _OnReturnCell;
            _scroller.Delegate = this;
            _lookAheadBefore = lookAheadBefore;
            _lookAheadAfter = lookAheadAfter;
            _container = container;
            _isFinishDisplay = false;
        }
        
        public void RegisterHeadSliderHint(GameObject headSliderHint)
        {
            _headSliderHint = headSliderHint;
        }
        
        public void RegisterTailSliderHint(GameObject tailSliderHint)
        {
            _tailSliderHint = tailSliderHint;
        }
        
        public virtual void Display(float normalizedScrollPosition = 0f)
        {
            _isFinishDisplay = false;
            _scroller.lookAheadBefore = _lookAheadBefore;
            _scroller.lookAheadAfter = _lookAheadAfter;
            _scroller.ReloadData(normalizedScrollPosition);
            _InitStartEndDataIndexAndHint();
            _isFinishDisplay = true;
        }
        
        public void RefreshActiveViews()
        {
            if (_isFinishDisplay)
            {
                _GetActiveViewsWithIndex().ForEach(pair => pair.view.Refresh(_dataModel.GetViewData(pair.index)));
            }
        }
        
        private List<(int index, SimpleEnhancedScrollerCellView view)> _GetActiveViewsWithIndex()
        {
            return (_scroller.NumberOfCells == 0)
                ? new List<(int, SimpleEnhancedScrollerCellView)>()
                : Enumerable.Range(_scroller.StartDataIndex, _scroller.EndDataIndex - _scroller.StartDataIndex + 1)
                    .Select(i => (index: i, view: _scroller.GetCellViewAtDataIndex(i) as SimpleEnhancedScrollerCellView))
                    .Where(pair => pair.view != null)
                    .ToList();
        }
        
        public virtual void Clear()
        {
            _dataModel.Clear();
            _scroller.ClearAll();
            _isFinishDisplay = false;
        }
        
        public float GetNormalizedScrollPosition()
        {
            return _scroller.NormalizedScrollPosition;
        }
        
        public float GetScrollPosition()
        {
            return _scroller.ScrollPosition;
        }
        
        public float NormalizedScrollPosition(float scrollPosition)
        {
            var predictScrollerSize = PredictItemScrollPosition(GetNumberOfCells(_scroller) - 1);
            return predictScrollerSize == 0f ? 0 : scrollPosition / predictScrollerSize;
        }
        
        public float PredictItemNormalizedScrollPosition(int dataIndex)
        {
            return NormalizedScrollPosition(PredictItemScrollPosition(dataIndex));
        }
        
        public float PredictItemScrollPosition(int dataIndex)
        {
            var sum = 0f;
            for(int i=0; i<dataIndex; i++)
            {
                sum += GetCellViewSize(_scroller, i);
            }
            return sum;
        }
        
        public void FocusToIndex(int dataIndex)
        {
            var scrollPosition = _scroller.GetScrollPositionForDataIndex(dataIndex, EnhancedScroller.CellViewPositionEnum.Before);
            FocusToPosition(scrollPosition);
        }
        
        public virtual void FocusToPosition(float scrollPosition)
        {
            _scroller.ReloadData();
            _scroller.SetScrollPositionImmediately(scrollPosition);
        }
        
        public void EnableDragging()
        {
            _scroller.EnableDragging();
        }
        
        public void DisableDragging()
        {
            _scroller.DisableDragging();
        }
        
        public EnhancedScrollerCellView GetViewAtIndex(int dataIndex)
        {
            return _scroller.GetCellViewAtDataIndex(dataIndex);
        }
        
        #region IEnhancedScrollerDelegate
        
        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return _dataModel.GetNumberOfCells();
        }
        
        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return _dataModel.GetCellViewSize(dataIndex);
        }

        public virtual EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var prefab = _dataModel.GetPrefabView(dataIndex);
            var cellView = scroller.GetCellView(prefab);
            var view = cellView as SimpleEnhancedScrollerCellView;
            var viewData = _dataModel.GetViewData(dataIndex);
            
            var onIndtantiateCell = _dataModel.GetOnInstantiateCell(dataIndex);
            onIndtantiateCell?.Invoke(view, viewData, dataIndex);
            
            view.Resolve(_container);
            _Display(view, viewData, dataIndex);
            
            return cellView;
        }
        
        #endregion
        
        protected virtual void _Display(SimpleEnhancedScrollerCellView view, IEnhancedScrollerElementViewData viewData, int dataIndex)
        {
            view.Display(viewData);
        }
        
        protected virtual void _OnReturnCell(EnhancedScrollerCellView cellView)
        {
            var dataIndex = cellView.dataIndex;
            var view = cellView as SimpleEnhancedScrollerCellView;
            var onReturnCell = _dataModel.GetOnReturnCell(dataIndex);
            onReturnCell?.Invoke(view);
            view.Leave();
        }
        
        protected void _InitStartEndDataIndexAndHint()
        {
            var startIndex = _GetScreenStartElementIndex();
            var endIndex = _GetScreenEndElementIndex();
            _UpdateStartEndDataIndex(startIndex, endIndex);
            _UpdateHint(startIndex, endIndex);
        }
        
        private void _OnScrollerScrolled(EnhancedScroller scroller, Vector2 val, float scrollPosition)
        {
            var startIndex = _GetScreenStartElementIndex();
            var endIndex = _GetScreenEndElementIndex();
            if(_isFinishDisplay && _scroller.NumberOfCells > 0 && (startIndex != _prevStartDataIndex || endIndex != _prevEndDataIndex))
            {
                _UpdateStartEndDataIndex(startIndex, endIndex);
                _UpdateHint(startIndex, endIndex);
                _PlayScrollingAudio();
            }
        }
        
        private void _UpdateStartEndDataIndex(int startIndex, int endIndex)
        {
            _prevStartDataIndex = startIndex;
            _prevEndDataIndex = endIndex;
        }
        
        private void _UpdateHint(int startIndex, int endIndex)
        {
            _headSliderHint?.SetActive(startIndex > 0);
            _tailSliderHint?.SetActive(endIndex < _scroller.NumberOfCells - 1);
        }
        
        protected int _GetScreenStartElementIndex()
        {
            var startPosition = _scroller._scrollPosition;
            return _scroller.GetCellViewIndexAtPosition(startPosition);
        }
        
        protected int _GetScreenEndElementIndex()
        {
            var endPosition = _scroller._scrollPosition + _scroller.ScrollRectSize;
            return _scroller.GetCellViewIndexAtPosition(endPosition);
        }
        
        public void UpdateScrollingAudioKey(string scrollingAudioKey)
        {
            _scrollingAudioKey = scrollingAudioKey;
        }

        private void _PlayScrollingAudio()
        {
            // _audioPresenter?.PlayGameAudio(_scrollingAudioKey);
        }
    }
}