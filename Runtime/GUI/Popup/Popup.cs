using System;
using UnityEngine;
using System.Collections.Generic;

namespace SisyphusFramework.GUI.Popup
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class Popup<TPopupName, TPopupItem> : MonoBehaviour 
        where TPopupName: struct, Enum
        where TPopupItem : APopupItem<TPopupName>

    {

        public abstract string Id { get; }
        

        /// <summary>
        /// Data container that is passed along to Screens that are being pushed. Screens can use these to setup
        /// themselves up with custom data provided at run-time.
        /// </summary>
        public class Data
        {
            private Dictionary<string, object> _data;

            public Data ()
            {
                _data = new Dictionary<string, object>();
            }

            public Data (int capacity)
            {
                _data = new Dictionary<string, object>(capacity);
            }

            public void Add(string key, object data)
            {
                _data.Add(key, data);
            }

            public T Get<T>(string key)
            {
                object datum = Get(key);

                try
                {
                    return (T)datum;
                }
                catch
                {
                    throw new System.Exception(string.Format("[BlitzyUI.Screen.Data] Could not cast data object '{0}' to type '{1}'", key, typeof(T).Name));
                }
            }

            public object Get (string key)
            {
                object datum;

                if (!_data.TryGetValue(key, out datum))
                    throw new System.Exception(string.Format("[BlitzyUI.Screen.Data] No object found for key '{0}'", key));

                return datum;
            }

            public bool TryGet (string key, out object datum)
            {
                return _data.TryGetValue(key, out datum);
            }

            public bool TryGet<T> (string key, out T datum)
            {
                object datumObj;

                if (_data.TryGetValue(key, out datumObj))
                {
                    try
                    {
                        datum = (T)datumObj;
                        return true;
                    }
                    catch
                    {
                        throw new System.Exception(string.Format("[BlitzyUI.Screen.Data] Could not cast data object '{0}' to type '{1}'", key, typeof(T).Name));
                    }
                }

                datum = default(T);
                return false;
            }
        }


        public string PrefabName { get; private set; }

        public bool keepCached = false;
        public bool overrideManagedSorting;
        public int overrideSortValue;

        public delegate void ScreenDelegate (Popup<TPopupName, TPopupItem> popup);

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
        public virtual void OnPush(Data data)
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
            PopupManager<TPopupName, TPopupItem>.Instance.QueuePop(null);
        }
    }
}