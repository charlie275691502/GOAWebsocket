using System.Collections;
using UnityEngine;
using Rayark.Mast;
using Zenject;

namespace EnhancedUI.EnhancedScroller.Internal
{
    public abstract class SimpleEnhancedScrollerCellView : EnhancedScrollerCellView
    {
        [SerializeField]
        protected GameObject _rootFolder;
        
        protected Executor _loadAssetExecutor = new Executor();
        
        private void Update()
        {
            _UpdateExecutor();
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
                
                _loadAssetExecutor.Clear();
                _loadAssetExecutor.Add(_LoadAsset(viewData));
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
            _loadAssetExecutor.Clear();
            _rootFolder.SetActive(false);
            _Unregister();
            _DisplayEmpty();
            _Leave();
        }
        
        protected virtual void _UpdateExecutor()
        {
            if (!_loadAssetExecutor.Empty)
            {
                _loadAssetExecutor.Resume(Time.deltaTime);
            }
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
        
        protected abstract IEnumerator _LoadAsset(IEnhancedScrollerElementViewData viewData);
        
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