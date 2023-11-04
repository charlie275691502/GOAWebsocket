using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller.Internal;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace EnhancedUI.EnhancedScroller
{
    public class SimpleEnhancedScrollerCellViewRow<T1, T2> : SimpleEnhancedScrollerCellView
        where T1 : SimpleEnhancedScrollerCellView
    {
        [SerializeField]
        protected List<T1> _views;
        
        public int ViewsCount()
        {
            return _views.Count;
        }
        
        public override sealed void Resolve(Zenject.DiContainer container)
        {
            _views.ForEach(view => view.Resolve(container));
            base.Resolve(container);
        }

        public override sealed void Display(IEnhancedScrollerElementViewData rawViewData)
        {
            base.Display(rawViewData);
            var viewData = rawViewData as EnhancedScrollerElementRowViewData<T2>;
            var viewDatas = viewData.ViewDatas;
            for(int i = 0; i < _views.Count; i++)
            {
                if (i < viewDatas.Count)
                {
                    _views[i].Display(viewDatas[i]);
                } else 
                {
                    _views[i].DisplayEmpty();
                }
            }
        }
        
        public override sealed void Refresh(IEnhancedScrollerElementViewData rawViewData)
        {
            base.Refresh(rawViewData);
            var viewData = rawViewData as EnhancedScrollerElementRowViewData<T2>;
            var viewDatas = viewData.ViewDatas;
            for(int i = 0; i < _views.Count; i++)
            {
                if (i < viewDatas.Count)
                {
                    _views[i].Refresh(viewDatas[i]);
                } else 
                {
                    _views[i].DisplayEmpty();
                }
            }
        }
        
        public override void Leave()
        {
            _views.ForEach(view => view.Leave());
            base.Leave();
        }
        
        public RectTransform GetRectTransform(int index)
        {
            return index < _views.Count ? _views[index].GetComponent<RectTransform>() : null;
        }
        
        protected override void _Display(IEnhancedScrollerElementViewData viewData)
        {
            _Display((EnhancedScrollerElementRowViewData<T2>)viewData);
        }
        
        protected override void _Refresh(IEnhancedScrollerElementViewData viewData)
        {
            _Refresh((EnhancedScrollerElementRowViewData<T2>)viewData);
        }
        
        protected override async UniTaskVoid _LoadAsset(IEnhancedScrollerElementViewData viewData, CancellationTokenSource token)
        {
            await _LoadAsset((EnhancedScrollerElementRowViewData<T2>)viewData, token);
        }
        
        protected virtual void _Display(EnhancedScrollerElementRowViewData<T2> viewData)
        {
            
        }
        
        protected virtual void _Refresh(EnhancedScrollerElementRowViewData<T2> viewData)
        {
            Display(viewData);
        }
        
        protected virtual async UniTask _LoadAsset(EnhancedScrollerElementRowViewData<T2> viewData, CancellationTokenSource token)
        {
            await UniTask.Yield();
        }
    }
}