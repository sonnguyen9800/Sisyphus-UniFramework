using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SisyphusFramework.Tutorial
{
    public abstract class ATutorialAction : UnityEngine.ScriptableObject
    {
        [HideInInspector]
        public string Description = "";

        #if UNITY_WEBGL
        public abstract UniTask Execute();
        public virtual UniTask OnFinished() => UniTask.CompletedTask;
        
        #else
        
        public abstract Task Execute();
        public virtual Task OnFinished() => Task.CompletedTask;
        #endif

    }
}