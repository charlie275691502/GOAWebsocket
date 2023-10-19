using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnhancedUI.EnhancedScroller
{
    public interface IEnhancedScrollerElementViewData
    {
        bool IsEmpty { get; }
    }
    
    public class EnhancedScrollerElementRowViewData<T> : IEnhancedScrollerElementViewData
    {
        public bool IsEmpty { get; }
        public bool HasEmpty { get; }
        public List<EnhancedScrollerElementViewData<T>> ViewDatas;
        
        private EnhancedScrollerElementRowViewData(int count)
        {
            IsEmpty = true;
            HasEmpty = true;
            ViewDatas = Enumerable.Repeat(EnhancedScrollerElementViewData<T>.CreateEmpty(), count).ToList();
        }
        
        public EnhancedScrollerElementRowViewData(List<T> viewDatas, int count)
        {
            IsEmpty = false;
            HasEmpty = viewDatas.Count < count;
            ViewDatas = viewDatas
                .Take(count)
                .Select(viewData => new EnhancedScrollerElementViewData<T>(viewData))
                .Concat(Enumerable.Repeat(EnhancedScrollerElementViewData<T>.CreateEmpty(), Mathf.Max(count - viewDatas.Count, 0)))
                .ToList();
        }
        
        public static EnhancedScrollerElementRowViewData<T> CreateEmpty(int count)
        {
            return new EnhancedScrollerElementRowViewData<T>(count);
        }
    }
    
    public class EnhancedScrollerElementViewData<T> : IEnhancedScrollerElementViewData
    {
        public bool IsEmpty { get; }
        public T Data;
        
        private static EnhancedScrollerElementViewData<T> Empty = new EnhancedScrollerElementViewData<T>();
        private EnhancedScrollerElementViewData()
        {
            IsEmpty = true;
        }
        
        public EnhancedScrollerElementViewData(T data)
        {
            IsEmpty = false;
            Data = data;
        }
        
        public static EnhancedScrollerElementViewData<T> CreateEmpty()
        {
            return Empty;
        }
    }
}