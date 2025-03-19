//#define PRINT_STACK
//#define PRINT_QUEUE
//#define PRINT_CACHE
//#define PRINT_FOCUS

using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityCommunity.UnitySingleton;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace SisyphusFramework.GUI.Popup
{
    public abstract class PopupManager<TPopupName, TPopupItem> : MonoSingleton<PopupManager<TPopupName, TPopupItem>>
        where TPopupName : struct, Enum 
        where TPopupItem : APopupItem<TPopupName>


    {
    public const string Version = "1.0.0";

    private abstract class QueuedScreen
    {
        public string PopupName;
    }


    private class QueuedScreenPush : QueuedScreen
    {
        public Popup<TPopupName, TPopupItem>.Data data;
        public string prefabName;
        public PushedDelegate callback;

        public override string ToString()
        {
            return string.Format("[Push] {0}", prefabName);
        }
    }


    private class QueuedScreenPop : QueuedScreen
    {
        public PoppedDelegate callback;

        public override string ToString()
        {
            return string.Format("[Pop] {0}", PopupName);
        }
    }

    public delegate void PushedDelegate(Popup<TPopupName, TPopupItem> popup);

    public delegate void PoppedDelegate(string name);


    [SerializeField] private APopupDatabase<TPopupName, TPopupItem> aPopupDatabase = null;
    public Canvas rootCanvas;

    /// <summary>
    /// [Ryan] A fix for input order not obeying the render order of the screens.
    /// This is a bug as of Unity 2019.1.9f1
    /// </summary>
    public bool inputOrderFixEnabled = true;

    private CanvasScaler _rootCanvasScalar;
    private Dictionary<string, Popup<TPopupName, TPopupItem>> _cache;
    private Queue<QueuedScreen> _queue;
    private List<Popup<TPopupName, TPopupItem>> _stack;
    private HashSet<string> _stackIdSet;
    private State _state;

    private PushedDelegate _activePushCallback;
    private PoppedDelegate _activePopCallback;



    private enum State
    {
        Ready,
        Push,
        Pop
    }

    protected override void Awake()
    {

        Debug.Log("[UIManager] Version: " + Version);

        // _rootCanvasScalar = rootCanvas.GetComponent<CanvasScaler>();
        // if (_rootCanvasScalar == null)
        // {
        //     throw new System.Exception(
        //         string.Format("{0} must have a CanvasScalar component attached to it for UIManager.", rootCanvas.name));
        // }

        _cache = new Dictionary<string, Popup<TPopupName, TPopupItem>>();
        _queue = new Queue<QueuedScreen>();
        _stack = new List<Popup<TPopupName, TPopupItem>>();
        _state = State.Ready;
        
    }



    /// <summary>
    /// Queue the screen to be pushed onto the screen stack. 
    /// Callback will be invoked when the screen is pushed to the stack.
    /// </summary>
    public void QueuePush(Popup<TPopupName, TPopupItem>.Data data, string prefabName = null, PushedDelegate callback = null)
    {
        string prefab = prefabName;
#if PRINT_QUEUE
            DebugPrintQueue(string.Format("[UIManager] QueuePush id: {0}, prefabName: {1}", id, prefab));
#endif

        if (GetScreen(prefabName) != null)
        {
            Debug.LogWarning(string.Format("Screen {0} already exists in the stack. Ignoring push request.", prefabName));
            return;
        }

        //if (ScreenWillExist(id))
        //{
        //    Debug.LogWarning(string.Format("Screen {0} will exist in the stack after the queue is fully executed. Ignoring push request.", id));
        //    return;
        //}

        QueuedScreenPush push = new QueuedScreenPush();
        push.data = data;
        push.prefabName = prefab;
        push.callback = callback;

        _queue.Enqueue(push);

#if PRINT_QUEUE
            DebugPrintQueue(string.Format("[UIManager] Enqueued Screen: {0}, Frame: {1}", push, Time.frameCount));
#endif

        if (CanExecuteNextQueueItem())
            ExecuteNextQueueItem();
    }

    /// <summary>
    /// Queue the screen to be popped from the screen stack. This will pop all screens on top of it as well.
    /// Callback will be invoked when the screen is reached, or popped if 'include' is true.
    /// </summary>
    public void QueuePopTo(string id, bool include, PoppedDelegate callback = null)
    {
#if PRINT_QUEUE
            DebugPrintQueue(string.Format("[UIManager] QueuePopTo id: {0}, include: {1}", id, include));
#endif

        bool found = false;

        for (int i = 0; i < _stack.Count; i++)
        {
            var screen = _stack[i];

            if (screen.Id != id)
            {
                var queuedPop = new QueuedScreenPop();
                queuedPop.PopupName = screen.PrefabName;

                _queue.Enqueue(queuedPop);

#if PRINT_QUEUE
                    DebugPrintQueue(string.Format("[UIManager] Enqueued Screen: {0}", queuedPop));
#endif
            }
            else
            {
                if (include)
                {
                    var queuedPop = new QueuedScreenPop();
                    queuedPop.PopupName = screen.PrefabName;
                    queuedPop.callback = callback;

                    _queue.Enqueue(queuedPop);

#if PRINT_QUEUE
                        DebugPrintQueue(string.Format("[UIManager] Enqueued Screen: {0}", queuedPop));
#endif
                }

                if (callback != null)
                    callback(screen.PrefabName);

                found = true;
                break;
            }
        }

        if (!found)
            Debug.LogWarning(string.Format("[UIManager] {0} was not in the stack. All screens have been popped.", id));

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

        Popup<TPopupName, TPopupItem> topPopup = GetTopScreen();
        if (topPopup == null)
            return;

        QueuedScreenPop pop = new QueuedScreenPop();
        pop.PopupName = topPopup.PrefabName;
        pop.callback = callback;

        _queue.Enqueue(pop);

#if PRINT_QUEUE
            DebugPrintQueue(string.Format("[UIManager] Enqueued Screen: {0}", pop));
#endif

        if (CanExecuteNextQueueItem())
            ExecuteNextQueueItem();
    }

    public void OnUpdate()
    {
        if (CanExecuteNextQueueItem())
            ExecuteNextQueueItem();
    }

    public Popup<TPopupName, TPopupItem> GetTopScreen()
    {
        if (_stack.Count > 0)
            return _stack[0];

        return null;
    }

    public Popup<TPopupName, TPopupItem> GetScreen(string id)
    {
        int count = _stack.Count;
        for (int i = 0; i < count; i++)
        {
            if (_stack[i].Id == id)
                return _stack[i];
        }

        return null;
    }

    public T GetScreen<T>(string id) where T : Popup<TPopupName, TPopupItem>
    {
        Popup<TPopupName, TPopupItem> popup = GetScreen(id);
        return (T)popup;
    }

    public void SetVisibility(bool visible)
    {
        var canvasGroup = rootCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = rootCanvas.gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = visible ? 1.0f : 0.0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }

    public bool IsVisible()
    {
        var canvasGroup = rootCanvas.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            return true;
        }

        bool isVisible = canvasGroup.alpha > 0.0f &&
                         canvasGroup.interactable == true &&
                         canvasGroup.blocksRaycasts == true;

        return isVisible;
    }

    private bool CanExecuteNextQueueItem()
    {
        if (_state == State.Ready)
        {
            if (_queue.Count > 0)
            {
                return true;
            }
        }

        return false;
    }

    private void ExecuteNextQueueItem()
    {
        // Get next queued item.
        QueuedScreen queued = _queue.Dequeue();

#if PRINT_QUEUE
            DebugPrintQueue(string.Format("[UIManager] Dequeued Screen: {0}, Frame: {1}", queued, Time.frameCount));
#endif

        if (queued is QueuedScreenPush)
        {
            // Push screen.
            QueuedScreenPush queuedPush = (QueuedScreenPush)queued;
            Popup<TPopupName, TPopupItem> popupInstance;

            if (_cache.TryGetValue(queuedPush.prefabName, out popupInstance))
            {
                // Use cached instance of screen.
                _cache.Remove(queuedPush.prefabName);

#if PRINT_CACHE
                    DebugPrintCache(string.Format("[UIManager] Screen retrieved from Cache: {0}", queuedPush.prefabName));
#endif

                // Move cached to the front of the transfrom heirarchy so that it is sorted properly.
                popupInstance.transform.SetAsLastSibling();

                popupInstance.gameObject.SetActive(true);
                StartCoroutine(FadeIn(popupInstance.gameObject));

            }
            else
            {

                if (Enum.TryParse(queuedPush.prefabName, out TPopupName popupName))
                {

                    var prefab = aPopupDatabase.GetByType(popupName);

                    if (prefab == null)
                    {
                        Debug.LogWarning("Game object of popup " + queuedPush.prefabName + " not existed");

                    }

                    popupInstance = Object.Instantiate(prefab, rootCanvas.transform).GetComponent<Popup<TPopupName, TPopupItem>>();
                    StartCoroutine(FadeIn(popupInstance.gameObject));
                    popupInstance.Setup(queuedPush.prefabName);
                }
                else
                {
                    Debug.LogWarning("Popup: " + queuedPush.prefabName + " not existed");
                }

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
            _stack.Insert(0, popupInstance);

            _activePushCallback = queuedPush.callback;

#if PRINT_STACK
                DebugPrintStack(string.Format("[UIManager] Pushing Screen: {0}, Frame: {1}", queued.id, Time.frameCount));
#endif

            popupInstance.onPushFinished += HandlePushFinished;
            popupInstance.OnPush(queuedPush.data);

            if (_queue.Count == 0)
            {
#if PRINT_FOCUS
                    Debug.Log(string.Format("[UIManager] Gained Focus: {0}", screenInstance.id));
#endif

                // Screen gains focus when it is on top of the screen stack and no other items in the queue.
                popupInstance.OnFocus();
            }
        }
        else
        {
            // Pop screen.
            QueuedScreenPop queuedPop = (QueuedScreenPop)queued;
            Popup<TPopupName, TPopupItem> popupToPop = GetTopScreen();

            if (popupToPop.Id != queued.PopupName)
            {
                throw new System.Exception(string.Format("The top screen does not match the queued pop. " +
                                                         "TopScreen: {0}, QueuedPop: {1}", popupToPop.PrefabName, queued.PopupName));
            }

#if PRINT_FOCUS
                Debug.Log(string.Format("[UIManager] Lost Focus: {0}", screenToPop.id));
#endif

            popupToPop.OnFocusLost();

            _state = State.Pop;
            _stack.RemoveAt(0);

            // Tell new top screen that it is gaining focus.
            var newTopScreen = GetTopScreen();
            if (newTopScreen != null)
            {
                if (_queue.Count == 0)
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
    }

    private void UpdateSortOrderOverrides()
    {
        int managedOrder = 0;

        int childCount = this.rootCanvas.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            var screen = this.rootCanvas.transform.GetChild(i).GetComponent<Popup<TPopupName, TPopupItem>>();
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

    /// <summary>
    /// Check to see if the screen will exist after the queue has been fully executed.
    /// </summary>
    //private bool ScreenWillExist (BlitzyUI.Screen.Id id)
    //{
    //    return false;

    //    // TODO: Infer if the screen will exists after the queue is fully executed.
    //}

    private void DebugPrintStack(string optionalEventMsg)
    {
        var sb = new System.Text.StringBuilder();

        if (!string.IsNullOrEmpty(optionalEventMsg))
            sb.AppendLine(optionalEventMsg);

        sb.AppendLine("[UIManager Screen Stack]");

        for (int i = 0; i < _stack.Count; i++)
        {
            sb.AppendLine(string.Format("{0}", _stack[i].PrefabName));
        }

        Debug.Log(sb.ToString());
    }

    private void DebugPrintQueue(string optionalEventMsg)
    {
        var sb = new System.Text.StringBuilder();

        if (!string.IsNullOrEmpty(optionalEventMsg))
            sb.AppendLine(optionalEventMsg);

        sb.AppendLine("[UIManager Screen Queue]");

        foreach (QueuedScreen queued in _queue)
        {
            sb.AppendLine(queued.ToString());
        }

        Debug.Log(sb.ToString());
    }

    private void DebugPrintCache(string optionalEventMsg)
    {
        var sb = new System.Text.StringBuilder();

        if (!string.IsNullOrEmpty(optionalEventMsg))
            sb.AppendLine(optionalEventMsg);

        sb.AppendLine("[UIManager Screen Cache]");

        foreach (KeyValuePair<string, Popup<TPopupName, TPopupItem>> cached in _cache)
        {
            sb.AppendLine(cached.Key);
        }

        Debug.Log(sb.ToString());
    }

    private void HandlePushFinished(Popup<TPopupName, TPopupItem> popup)
    {
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

    private void HandlePopFinished(Popup<TPopupName, TPopupItem> popup)
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

    public float fadeDuration = 0.3f; // Duration for fade-in and fade-out


    
    private IEnumerator FadeIn(GameObject screen)
    {
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>() ?? screen.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        screen.SetActive(true);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
    }

    #if UNITY_WEBGL
    private async void FadeOutAndDestroy(GameObject screen)
    {
        UniTaskCompletionSource<bool> tcs = new UniTaskCompletionSource<bool>();
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>() ?? screen.AddComponent<CanvasGroup>();
        // canvasGroup.DOFade(0, fadeDuration).OnComplete(() =>
        // {
        //     Destroy(screen);
        //     _state = State.Ready;
        //     tcs.TrySetResult(true);
        // });

        await tcs.Task;

    }
    #else
        private async void FadeOutAndDestroy(GameObject screen)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>() ?? screen.AddComponent<CanvasGroup>();
        canvasGroup.DOFade(0, fadeDuration).OnComplete(() =>
        {
            Destroy(screen);
            _state = State.Ready;
            tcs.TrySetResult(true);
        });

        await tcs.Task;

    }
#endif

    }
}