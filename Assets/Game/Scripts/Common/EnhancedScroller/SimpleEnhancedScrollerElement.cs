using System.Threading;
using Cysharp.Threading.Tasks;
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
        
        protected override async UniTaskVoid _LoadAsset(IEnhancedScrollerElementViewData viewData, CancellationTokenSource token)
        {
            await _LoadAsset(((EnhancedScrollerElementViewData<T>)viewData).Data, token);
        }
        
        protected virtual void _Display(T viewData)
        {
            
        }
        
        protected virtual void _Refresh(T viewData)
        {
            Display(new EnhancedScrollerElementViewData<T>(viewData));
        }
        
        protected virtual async UniTask _LoadAsset(T viewData, CancellationTokenSource token)
        {
            await UniTask.Yield();
        }
    }
}