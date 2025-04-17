//#define PRINT_STACK
//#define PRINT_QUEUE
//#define PRINT_CACHE
//#define PRINT_FOCUS

using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using UnityCommunity.UnitySingleton;
using Object = UnityEngine.Object;

namespace SisyphusFramework.GUI.Popup
{
    public abstract partial class APopupManager : MonoSingleton<APopupManager>
    {
        [SerializeField] private UnityEngine.ScriptableObject _popupDatabase;
        private IPopupDatabase aPopupDatabase = null;
        public Canvas rootCanvas;

        /// <summary>
        /// [Ryan] A fix for input order not obeying the render order of the screens.
        /// This is a bug as of Unity 2019.1.9f1
        /// </summary>
        public bool inputOrderFixEnabled = true;

        private CanvasScaler _rootCanvasScalar;
        private Dictionary<string, Popup> _cache;
        private Queue<QueuedAction> _queueAction;
        private List<Popup> _stackPopup;
        private HashSet<string> _stackIdSet;
        private State _state;

        private PushedDelegate _activePushCallback;
        private PoppedDelegate _activePopCallback;


        protected override void Awake()
        {

            aPopupDatabase = _popupDatabase as IPopupDatabase;
            _cache = new Dictionary<string, Popup>();
            _queueAction = new Queue<QueuedAction>();
            _stackPopup = new List<Popup>();
            _state = State.Ready;
        }


        /// <summary>
        /// Queue the screen to be pushed onto the screen stack. 
        /// Callback will be invoked when the screen is pushed to the stack.
        /// </summary>
        public void QueuePush(PopupParamData data, string prefabName = null,
            PushedDelegate callback = null)
        {
            string prefab = prefabName;
#if PRINT_QUEUE
            DebugPrintQueue(string.Format("[UIManager] QueuePush id: {0}, prefabName: {1}", id, prefab));
#endif

            if (GetScreen(prefabName) != null)
            {
                Debug.LogWarning(string.Format("Screen {0} already exists in the stack. Ignoring push request.",
                    prefabName));
                return;
            }

            //if (ScreenWillExist(id))
            //{
            //    Debug.LogWarning(string.Format("Screen {0} will exist in the stack after the queue is fully executed. Ignoring push request.", id));
            //    return;
            //}

            QueuedActionPush push = new QueuedActionPush();
            push.Data = data;
            push.PrefabName = prefab;
            push.Callback = callback;

            _queueAction.Enqueue(push);

#if PRINT_QUEUE
            DebugPrintQueue(string.Format("[UIManager] Enqueued Screen: {0}, Frame: {1}", push, Time.frameCount));
#endif

            if (CanExecuteNextQueueItem())
                ExecuteNextQueueItem();
        }

        /// <summary>
        /// Queue the top-most screen to be popped from the screen stack.
        /// Callback will be invoked when the screen is popped from the stack.
        /// </summary>
        public void QueuePop(PoppedDelegate callback = null)
        {
#if PRINT_QUEUE
            DebugPrintQueue(string.Format("[UIManager] QueuePop"));
#endif

            Popup topPopup = GetTopScreen();
            if (topPopup == null)
                return;

            QueuedActionPop pop = new QueuedActionPop();
            pop.PopupName = topPopup.PrefabName;
            pop.callback = callback;

            _queueAction.Enqueue(pop);

#if PRINT_QUEUE
            DebugPrintQueue(string.Format("[UIManager] Enqueued Screen: {0}", pop));
#endif

            if (CanExecuteNextQueueItem())
                ExecuteNextQueueItem();
        }

        private void Update()
        {
            OnUpdate();
        }

        public void OnUpdate()
        {
            if (CanExecuteNextQueueItem())
                ExecuteNextQueueItem();
        }

        public Popup GetTopScreen()
        {
            Debug.LogError("Current Stack Count: " + _stackPopup.Count);
            if (_stackPopup.Count > 0)
                return _stackPopup[0];

            return null;
        }

        public Popup GetScreen(string id)
        {
            int count = _stackPopup.Count;
            for (int i = 0; i < count; i++)
            {
                if (_stackPopup[i].Id == id)
                    return _stackPopup[i];
            }

            return null;
        }



        private bool CanExecuteNextQueueItem()
        {
            if (_state == State.Ready)
            {
                if (_queueAction.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void HandleQueuePush(QueuedAction queued)
        {
            // Push screen.
            QueuedActionPush queuedPush = (QueuedActionPush)queued;
            Popup popupInstance;

            if (_cache.TryGetValue(queuedPush.PrefabName, out popupInstance))
            {
                // Use cached instance of screen.
                _cache.Remove(queuedPush.PrefabName);

#if PRINT_CACHE
                    DebugPrintCache(string.Format("[UIManager] Screen retrieved from Cache: {0}", queuedPush.prefabName));
#endif

                // Move cached to the front of the transfrom heirarchy so that it is sorted properly.
                popupInstance.transform.SetAsLastSibling();

                popupInstance.gameObject.SetActive(true);
                //popupInstance.Setup(queuedPush.PrefabName);
                FadeIn(popupInstance.gameObject, () => { popupInstance.Setup(queuedPush.PrefabName); });
            }
            else
            {
                var prefab = aPopupDatabase.GetPopupByName(queuedPush.PrefabName);

                if (prefab == null)
                {
                    Debug.LogWarning("Game object of popup " + queuedPush.PrefabName + " not existed");
                }

                popupInstance = Object.Instantiate(prefab, rootCanvas.transform)
                    .GetComponent<Popup>();
                //popupInstance.Setup(queuedPush.PrefabName);
                FadeIn(popupInstance.gameObject, () => { popupInstance.Setup(queuedPush.PrefabName); });
            }

            if (this.inputOrderFixEnabled)
            {
                this.UpdateSortOrderOverrides();
            }

            // Tell previous top screen that it is losing focus.
            var topScreen = GetTopScreen();
            if (topScreen != null)
            {
#if PRINT_FOCUS
                    Debug.Log(string.Format("[UIManager] Lost Focus: {0}", topScreen.id));
#endif

                topScreen.OnFocusLost();
            }

            // Insert new screen at the top of the stack.
            _state = State.Push;
            _stackPopup.Insert(0, popupInstance);

            _activePushCallback = queuedPush.Callback;

#if PRINT_STACK
                DebugPrintStack(string.Format("[UIManager] Pushing Screen: {0}, Frame: {1}", queued.id, Time.frameCount));
#endif

            popupInstance.onPushFinished += HandlePushFinished;
            popupInstance.OnPush(queuedPush.Data);

            if (_queueAction.Count == 0)
            {
#if PRINT_FOCUS
                    Debug.Log(string.Format("[UIManager] Gained Focus: {0}", screenInstance.id));
#endif

                // Screen gains focus when it is on top of the screen stack and no other items in the queue.
                popupInstance.OnFocus();
            }
        }

        private void HandleQueuePop(QueuedAction queued)
        {
            Debug.Log("Handle Queue Pop"  + queued.PopupName);
            // Pop screen.
            QueuedActionPop queuedPop = (QueuedActionPop)queued;
            Popup popupToPop = GetTopScreen();

            if (popupToPop.Id != queued.PopupName)
            {
                throw new Exception(string.Format("The top screen does not match the queued pop. " +
                                                  "TopScreen: {0}, QueuedPop: {1}", popupToPop.PrefabName,
                    queued.PopupName));
            }

#if PRINT_FOCUS
                Debug.Log(string.Format("[UIManager] Lost Focus: {0}", screenToPop.id));
#endif

            popupToPop.OnFocusLost();

            _state = State.Pop;
            _stackPopup.RemoveAt(0);

            // Tell new top screen that it is gaining focus.
            var newTopScreen = GetTopScreen();
            if (newTopScreen != null)
            {
                if (_queueAction.Count == 0)
                {
#if PRINT_FOCUS
                        Debug.Log(string.Format("[UIManager] Gained Focus: {0}", newTopScreen.id));
#endif

                    // Screen gains focus when it is on top of the screen stack and no other items in the queue.
                    newTopScreen.OnFocus();
                }
            }

            _activePopCallback = queuedPop.callback;

#if PRINT_STACK
                DebugPrintStack(string.Format("[UIManager] Popping Screen: {0}, Frame: {1}", queued.id, Time.frameCount));
#endif

            popupToPop.onPopFinished += HandlePopFinished;
            popupToPop.OnPop();
        }

        private void ExecuteNextQueueItem()
        {
            // Get next queued item.
            QueuedAction queued = _queueAction.Dequeue();

#if PRINT_QUEUE
            DebugPrintQueue(string.Format("[UIManager] Dequeued Screen: {0}, Frame: {1}", queued, Time.frameCount));
#endif

            if (queued is QueuedActionPush)
            {
                HandleQueuePush(queued);
            }
            else if (queued is QueuedActionPop)
            {
               HandleQueuePop(queued);
            }
        }

        private void UpdateSortOrderOverrides()
        {
            int managedOrder = 0;

            int childCount = this.rootCanvas.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var screen = this.rootCanvas.transform.GetChild(i).GetComponent<Popup>();
                if (screen != null)
                {
                    var canvas = screen.GetComponent<Canvas>();
                    if (canvas != null)
                    {
                        canvas.overrideSorting = true;

                        if (screen.overrideManagedSorting)
                        {
                            canvas.sortingOrder = screen.overrideSortValue;
                        }
                        else
                        {
                            canvas.sortingOrder = managedOrder;
                            managedOrder++;
                        }
                    }
                }
            }
        }
        

        private void HandlePushFinished(Popup popup)
        {
            Debug.LogError("Handle Push Finished");
            popup.onPushFinished -= HandlePushFinished;

            _state = State.Ready;

            if (_activePushCallback != null)
            {
                _activePushCallback(popup);
                _activePushCallback = null;
            }

            if (CanExecuteNextQueueItem())
                ExecuteNextQueueItem();
        }

        private void HandlePopFinished(Popup popup)
        {
            popup.onPopFinished -= HandlePopFinished;

            if (popup.keepCached)
            {
                // Store in the cache for later use.
                popup.gameObject.SetActive(false);

                // TODO: Need to have a better cache storage mechanism that supports multiple screens of the same prefab?
                if (!_cache.ContainsKey(popup.PrefabName))
                {
                    _cache.Add(popup.PrefabName, popup);

#if PRINT_CACHE
                    DebugPrintCache(string.Format("[UIManager] Screen added to Cache: {0}", screen.PrefabName));
#endif
                }
            }
            else
            {
                // Destroy screen.
                FadeOutAndDestroy(popup.gameObject);
                //SetStateReady(popup.gameObject);
                //tcs.TrySetResult(true);
            }

            _state = State.Ready;

            if (_activePopCallback != null)
            {
                _activePopCallback(popup.PrefabName);
                _activePopCallback = null;
            }

            if (CanExecuteNextQueueItem())
                ExecuteNextQueueItem();
        }

        


        protected void SetStateReady(GameObject screen)
        {
            Destroy(screen);

            _state = State.Ready;
        }

        protected abstract void FadeOutAndDestroy(GameObject screen);
        protected abstract void FadeIn(GameObject screen, Action onComplete);
    }
}