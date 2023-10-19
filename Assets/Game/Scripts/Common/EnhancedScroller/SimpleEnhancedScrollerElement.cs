using System.Collections;
using EnhancedUI.EnhancedScroller.Internal;

namespace EnhancedUI.EnhancedScroller
{
    public class SimpleEnhancedScrollerElement<T> : SimpleEnhancedScrollerCellView
    {
        protected override void _Display(IEnhancedScrollerElementViewData viewData)
        {
            _Display(((EnhancedScrollerElementViewData<T>)viewData).Data);
        }
        
        protected override void _Refresh(IEnhancedScrollerElementViewData viewData)
        {
            _Refresh(((EnhancedScrollerElementViewData<T>)viewData).Data);
        }
        
        protected override IEnumerator _LoadAsset(IEnhancedScrollerElementViewData viewData)
        {
            yield return _LoadAsset(((EnhancedScrollerElementViewData<T>)viewData).Data);
        }
        
        protected virtual void _Display(T viewData)
        {
            
        }
        
        protected virtual void _Refresh(T viewData)
        {
            Display(new EnhancedScrollerElementViewData<T>(viewData));
        }
        
        protected virtual IEnumerator _LoadAsset(T viewData)
        {
            yield break;
        }
    }
}