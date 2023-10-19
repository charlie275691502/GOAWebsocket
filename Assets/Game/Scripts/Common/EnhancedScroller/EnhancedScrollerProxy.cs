using UnityEngine.UI;

namespace EnhancedUI.EnhancedScroller
{
    public class EnhancedScrollerProxy : EnhancedScroller
    {
        public void EnableDragging()
        {
            var scrollRect = ScrollRect ?? this.GetComponent<ScrollRect>();
            scrollRect.horizontal = scrollDirection == ScrollDirectionEnum.Horizontal;
            scrollRect.vertical = scrollDirection == ScrollDirectionEnum.Vertical;
        }
        
        public void DisableDragging()
        {
            var scrollRect = ScrollRect ?? this.GetComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = false;
        }
        
        // Check if EnhancedScroller had already initialized
        public override void ReloadData(float scrollPositionFactor = 0)
        {
            if(!_initialized)
            {
                _Initialize();
                _Register();
            }
            
            base.ReloadData(scrollPositionFactor);
        }
    }
}