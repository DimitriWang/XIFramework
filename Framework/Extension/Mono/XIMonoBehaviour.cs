using System.Collections.Generic;
using UnityEngine;

namespace XIFramework
{
    public class XIMonoBehaviour : MonoBehaviour
    {
        public virtual void Show()
        {
            gameObject.SetActive(true);
            
            OnShow();
        }

        public virtual void Hide()
        {
            OnHide();
            
            gameObject.SetActive(false);
        }
        public virtual void OnShow() { }
        
        public virtual void OnHide() { }
        
        
        protected virtual void OnDestroy()
        {			
            if (Application.isPlaying) 
            {
                OnBeforeDestroy();
                UnRegisterAllEvent();
            }
        }
        
        private List<ushort> mPrivateEventIds = null;
		
        private List<ushort> mCachedEventIds
        {
            get { return mPrivateEventIds ?? (mPrivateEventIds = new List<ushort>()); }
        }
        
        protected void UnRegisterAllEvent()
        {
            if (null != mPrivateEventIds)
            {
               // mPrivateEventIds.ForEach(id => Manager.UnRegisterEvent(id,Process));
            }
        }
		
        protected virtual void OnBeforeDestroy(){}
    }
}