using System;
using UnityEngine;
using System.Collections.Generic;

namespace SisyphusFramework.GUI.Popup
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class Popup : MonoBehaviour 
    {
        public abstract string Id { get; }
        
        public string PrefabName { get; private set; }

        public bool keepCached = false;
        public bool overrideManagedSorting;
        public int overrideSortValue;
        
        public event ScreenDelegate onPushFinished;
        public event ScreenDelegate onPopFinished;

        public void Setup(string prefabName)
        {
            PrefabName = prefabName;

            OnSetup();
        }

        /// <summary>
        /// Setup is called after instantiating a Screen prefab. It is only called once for the lifecycle of the Screen.
        /// </summary>
        public abstract void OnSetup();

        /// <summary>
        /// Called by the UIManager when this Screen is being pushed to the screen stack.
        /// Be sure to call PushPopFinished when your screen is done pushing. Delaying the PushPopFinished call
        /// allows the screen to delay execution of the UIManager's screen queue.
        /// </summary>
        public virtual void OnPush(PopupParamData popupParamData)
        {
            PushFinished();
        }

        /// <summary>
        /// Called by the UIManager when this Screen is being popped from the screen stack.
        /// Be sure to call PopFinished when your screen is done popping. Delaying the PushPopFinished call
        /// allows the screen to delay execution of the UIManager's screen queue.
        /// </summary>
        public virtual void OnPop()
        {
            PopFinished();
        }

        /// <summary>
        /// Called by the UIManager when this Screen becomes the top most screen in the stack.
        /// </summary>
        public abstract void OnFocus();

        /// <summary>
        /// Called by the UIManager when this Screen is no longer the top most screen in the stack.
        /// </summary>
        public abstract void OnFocusLost();

        protected void PushFinished ()
        {
            if (onPushFinished != null)
                onPushFinished(this);
            
        }

        protected void PopFinished ()
        {
            if (onPopFinished != null)
                onPopFinished(this);
        }

        public void Close()
        {
            //APopupManager<TPopupName, TPopupItem>.Instance.QueuePop(null);
        }
    }
}