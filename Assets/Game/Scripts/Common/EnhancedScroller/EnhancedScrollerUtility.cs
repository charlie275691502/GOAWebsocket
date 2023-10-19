using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnhancedUI.EnhancedScroller
{
    public class EnhancedScrollerUtility
    {
        public static List<EnhancedScrollerElementRowViewData<T>> GetRowViewDataList<T>(List<T> viewDatas, int countPerRow, int minRowCount = 0)
        {
            int count = (viewDatas.Count + countPerRow - 1) / countPerRow;
            return Enumerable.Range(0, Mathf.Max(count, minRowCount))
                .Select(i => (i < count) 
                        ? new EnhancedScrollerElementRowViewData<T>(viewDatas.Skip(i * countPerRow).Take(countPerRow).ToList(), countPerRow)
                        : EnhancedScrollerElementRowViewData<T>.CreateEmpty(countPerRow))
                .ToList();
        }
        
        public static List<EnhancedScrollerElementViewData<T>> GetViewDataList<T>(List<T> viewDatas, int minCount = 0)
        {
            return viewDatas
                .Select(viewData => new EnhancedScrollerElementViewData<T>(viewData))
                .Concat(Enumerable.Repeat(
                    EnhancedScrollerElementViewData<T>.CreateEmpty(),
                    Mathf.Max(0, minCount - viewDatas.Count)))
                .ToList();
        }
    }
}