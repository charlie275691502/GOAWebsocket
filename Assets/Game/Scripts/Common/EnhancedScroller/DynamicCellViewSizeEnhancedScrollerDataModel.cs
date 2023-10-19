using System;
using System.Collections.Generic;
using UnityEngine;

namespace EnhancedUI.EnhancedScroller
{
    public class DynamicCellViewSizeEnhancedScrollerDataModel<T1, T2> : EnhancedScrollerDataModel<T1, T2>
        where T1 : EnhancedScrollerCellView
        where T2 : IEnhancedScrollerElementViewData
    {
        private Dictionary<int, float> cellViewSizeDictionary = new Dictionary<int, float>();
        
        public DynamicCellViewSizeEnhancedScrollerDataModel(GameObject prefab, EnhancedScroller.ScrollDirectionEnum scrollDirection)
            : base(prefab, scrollDirection)
        {
            
        }
        
        public override float GetCellViewSize(int dataIndex)
        {
            return cellViewSizeDictionary.TryGetValue(dataIndex, out var value) ? value : _cellViewSize;
        }
        
        public override void Clear()
        {
            base.Clear();
            ClearCellViewSizeDictionary();
        }
        
        public void UpdateCellViewSizeDictionary(int index, float cellViewSize)
        {
            cellViewSizeDictionary[index] = cellViewSize;
        }
        
        public void ClearCellViewSizeDictionary()
        {
            cellViewSizeDictionary.Clear();
        }
    }
}