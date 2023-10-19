using System;
using System.Collections.Generic;
using UnityEngine;

namespace EnhancedUI.EnhancedScroller
{
    public class DoublePrefabEnhancedScrollerDataModel<T1, T2, T3, T4> : IEnhancedScrollerDataModel 
        where T1 : EnhancedScrollerCellView
        where T2 : IEnhancedScrollerElementViewData
        where T3 : EnhancedScrollerCellView
        where T4 : IEnhancedScrollerElementViewData
    {
        private enum ViewType
        {
            First,
            Second,
        }
        
        private List<T2> _firstViewDatas = new List<T2>();
        private List<T4> _secondViewDatas = new List<T4>();
        private EnhancedScrollerCellView _firstPrefabView;
        private EnhancedScrollerCellView _secondPrefabView;
        private float _firstCellViewSize;
        private float _secondCellViewSize;
        
        public event Action<T1, T2, int> OnInstantiateFirstCell;
        public event Action<T3, T4, int> OnInstantiateSecondCell;
        public event Action<T1> OnReturnFirstCell;
        public event Action<T3> OnReturnSecondCell;
        private Action<EnhancedScrollerCellView, IEnhancedScrollerElementViewData, int> onInstantiateFirstCell;
        private Action<EnhancedScrollerCellView, IEnhancedScrollerElementViewData, int> onInstantiateSecondCell;
        private Action<EnhancedScrollerCellView> onReturnFirstCell;
        private Action<EnhancedScrollerCellView> onReturnSecondCell;
        
        public DoublePrefabEnhancedScrollerDataModel(GameObject firstPrefab, GameObject secondPrefab, EnhancedScroller.ScrollDirectionEnum scrollDirection)
        {
            _firstPrefabView = firstPrefab.GetComponent<EnhancedScrollerCellView>();
            _secondPrefabView = secondPrefab.GetComponent<EnhancedScrollerCellView>();
            _firstPrefabView.cellIdentifier = firstPrefab.name;
            _secondPrefabView.cellIdentifier = secondPrefab.name;
            _firstCellViewSize = (scrollDirection == EnhancedScroller.ScrollDirectionEnum.Horizontal) 
                ? firstPrefab.GetComponent<RectTransform>().rect.width 
                : firstPrefab.GetComponent<RectTransform>().rect.height;
            _secondCellViewSize = (scrollDirection == EnhancedScroller.ScrollDirectionEnum.Horizontal) 
                ? secondPrefab.GetComponent<RectTransform>().rect.width 
                : secondPrefab.GetComponent<RectTransform>().rect.height;
            onInstantiateFirstCell = (view, viewData, index) => OnInstantiateFirstCell?.Invoke(view as T1, (T2)viewData, index);
            onInstantiateSecondCell = (view, viewData, index) => OnInstantiateSecondCell?.Invoke(view as T3, (T4)viewData, index);
            onReturnFirstCell = (view) => OnReturnFirstCell?.Invoke(view as T1);
            onReturnSecondCell = (view) => OnReturnSecondCell?.Invoke(view as T3);
        }
        
        public void UpdateViewDatas(List<T2> firstViewDatas, List<T4> secondViewDatas)
        {
            _firstViewDatas = firstViewDatas;
            _secondViewDatas = secondViewDatas;
        }
        
        public int GetNumberOfCells()
        {
            return _firstViewDatas.Count + _secondViewDatas.Count;
        }
        
        public float GetCellViewSize(int dataIndex)
        {
            return _GetViewType(dataIndex) switch
            {
                ViewType.First => _firstCellViewSize,
                ViewType.Second => _secondCellViewSize,
                _ => 0f
            };
        }
        
        public EnhancedScrollerCellView GetPrefabView(int dataIndex)
        {
            return _GetViewType(dataIndex) switch
            {
                ViewType.First => _firstPrefabView,
                ViewType.Second => _secondPrefabView,
                _ => null
            };
        }
        
        public IEnhancedScrollerElementViewData GetViewData(int dataIndex)
        {
            return _GetViewType(dataIndex) switch
            {
                ViewType.First => _firstViewDatas[dataIndex] as IEnhancedScrollerElementViewData,
                ViewType.Second => _secondViewDatas[dataIndex - _firstViewDatas.Count] as IEnhancedScrollerElementViewData,
                _ => null
            };
        }
        
        public Action<EnhancedScrollerCellView, IEnhancedScrollerElementViewData, int> GetOnInstantiateCell(int dataIndex)
        {
            return _GetViewType(dataIndex) switch
            {
                ViewType.First => onInstantiateFirstCell,
                ViewType.Second => onInstantiateSecondCell,
                _ => null
            };
        }
        
        public Action<EnhancedScrollerCellView> GetOnReturnCell(int dataIndex)
        {
            return _GetViewType(dataIndex) switch
            {
                ViewType.First => onReturnFirstCell,
                ViewType.Second => onReturnSecondCell,
                _ => null
            };
        }
        
        public void Clear()
        {
            _firstViewDatas.Clear();
            _secondViewDatas.Clear();
        }
        
        private ViewType _GetViewType(int dataIndex)
        {
            return (dataIndex < _firstViewDatas.Count) ? ViewType.First : ViewType.Second;
        }
    }
}