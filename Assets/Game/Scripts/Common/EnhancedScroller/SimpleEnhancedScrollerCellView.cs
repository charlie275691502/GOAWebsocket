using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Zenject;

namespace EnhancedUI.EnhancedScroller.Internal
{
    public abstract class SimpleEnhancedScrollerCellView : EnhancedScrollerCellView
    {
        [SerializeField]
        protected GameObject _rootFolder;

        private CancellationTokenSource _loadAssetToken;

        private void Update()
        {
            _Update();
        }
        
        public virtual void Resolve(DiContainer container)
        {
            _Resolve(container);
            _Register();
        }

        public virtual void Display(IEnhancedScrollerElementViewData viewData)
        {
            if(viewData.IsEmpty)
            {
                DisplayEmpty();
            } else 
            {
                _rootFolder.SetActive(true);
                _Display(viewData);

                _loadAssetToken?.Cancel();
                _loadAssetToken = new CancellationTokenSource();
                _LoadAsset(viewData, _loadAssetToken);
            }
        }
        
        public void DisplayEmpty()
        {
            _rootFolder.SetActive(true);
            _DisplayEmpty();
        }
        
        public virtual void Refresh(IEnhancedScrollerElementViewData viewData)
        {
            if(viewData.IsEmpty)
            {
                DisplayEmpty();
            } else 
            {
                _Refresh(viewData);
            }
        }
        
        public virtual void Leave()
        {
            _loadAssetToken?.Cancel();
            _rootFolder.SetActive(false);
            _Unregister();
            _DisplayEmpty();
            _Leave();
        }
        
        #region Override by child
        
        protected virtual void _Update()
        {
            
        }
        
        protected virtual void _Resolve(DiContainer container)
        {
            
        }
        
        protected abstract void _Display(IEnhancedScrollerElementViewData viewData);
        
        protected virtual void _DisplayEmpty()
        {
            
        }
        
        protected abstract void _Refresh(IEnhancedScrollerElementViewData viewData);
        
        protected abstract UniTaskVoid _LoadAsset(IEnhancedScrollerElementViewData viewData, CancellationTokenSource token);
        
        protected virtual void _Leave()
        {
            
        }
        
        protected virtual void _Register()
        {
            
        }
        
        protected virtual void _Unregister()
        {
            
        }
        
        #endregion
    }
}