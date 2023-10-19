using System;
using System.Collections.Generic;
using UnityEngine;

namespace EnhancedUI.EnhancedScroller
{
    public interface IEnhancedScrollerDataModel
    {
        int GetNumberOfCells();
        float GetCellViewSize(int dataIndex);
        EnhancedScrollerCellView GetPrefabView(int dataIndex);
        IEnhancedScrollerElementViewData GetViewData(int dataIndex);
        Action<EnhancedScrollerCellView, IEnhancedScrollerElementViewData, int> GetOnInstantiateCell(int dataIndex);
        Action<EnhancedScrollerCellView> GetOnReturnCell(int dataIndex);
        void Clear();
    }
    
    public class EnhancedScrollerDataModel<T1, T2> : IEnhancedScrollerDataModel
        where T1 : EnhancedScrollerCellView
        where T2 : IEnhancedScrollerElementViewData
    {
        private List<T2> _viewDatas = new List<T2>();
        private EnhancedScrollerCellView _prefabView;
        protected float _cellViewSize;
        
        public event Action<T1, T2, int> OnInstantiateCell;
        public event Action<T1> OnReturnCell;
        private Action<EnhancedScrollerCellView, IEnhancedScrollerElementViewData, int> onInstantiateCell;
        private Action<EnhancedScrollerCellView> onReturnCell;
        public EnhancedScrollerDataModel(GameObject prefab, EnhancedScroller.ScrollDirectionEnum scrollDirection)
        {
            _prefabView = prefab.GetComponent<EnhancedScrollerCellView>();
            _prefabView.cellIdentifier = prefab.name;
            _cellViewSize = (scrollDirection == EnhancedScroller.ScrollDirectionEnum.Horizontal) 
                ? prefab.GetComponent<RectTransform>().rect.width 
                : prefab.GetComponent<RectTransform>().rect.height;
            onInstantiateCell = (view, viewData, index) => OnInstantiateCell?.Invoke(view as T1, (T2)viewData, index);
            onReturnCell = (view) => OnReturnCell?.Invoke(view as T1);
        }
        
        public void UpdateViewDatas(List<T2> viewDatas)
        {
            _viewDatas = viewDatas;
        }
        
        public int GetNumberOfCells()
        {
            return _viewDatas.Count;
        }
        
        public virtual float GetCellViewSize(int dataIndex)
        {
            return _cellViewSize;
        }
        
        public EnhancedScrollerCellView GetPrefabView(int dataIndex)
        {
            return _prefabView;
        }
        
        public IEnhancedScrollerElementViewData GetViewData(int dataIndex)
        {
            return _viewDatas[dataIndex];
        }
        
        public Action<EnhancedScrollerCellView, IEnhancedScrollerElementViewData, int> GetOnInstantiateCell(int dataIndex)
        {
            return onInstantiateCell;
        }
        
        public Action<EnhancedScrollerCellView> GetOnReturnCell(int dataIndex)
        {
            return onReturnCell;
        }
        
        public virtual void Clear()
        {
            _viewDatas.Clear();
        }
    }
}