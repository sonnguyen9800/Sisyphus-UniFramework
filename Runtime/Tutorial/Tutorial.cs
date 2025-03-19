using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SisyphusFramework.Tutorial
{
    [CreateAssetMenu(menuName = "Tutorial/Main")]
    public class Tutorial : UnityEngine.ScriptableObject
    {
        
        public TutorialStep[] steps;
        private CancellationTokenSource _cancellationTokenSource;


        public void SetCancellationToken(CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;
        }
        private void SetupStepsKey()
        {
            for (byte i = 0; i < steps.Length; i++)
            {
                steps[i].Key = i.ToString();
            }
        }

        private Action _onTUtorialCompleted = null;
        
        
        #if UNITY_WEBGL
        
        public async UniTask StartTutorial(
            Action onTutorialStartCb = null,
            Action onTutorialCompletedCb = null)
        {
            SetupStepsKey();
            _onTUtorialCompleted = onTutorialCompletedCb;

            onTutorialStartCb?.Invoke();
            
            foreach (var step in steps)
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                    break;
                await step.Execute(_cancellationTokenSource.Token);
            }

            onTutorialCompletedCb?.Invoke();
        }
    
        
        #else
            public async Task StartTutorial(
            Action onTutorialStartCb = null,
            Action onTutorialCompletedCb = null)
        {
            SetupStepsKey();
            _onTUtorialCompleted = onTutorialCompletedCb;

            onTutorialStartCb?.Invoke();
            
            foreach (var step in steps)
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                    break;
                await step.Execute(_cancellationTokenSource.Token);
            }

            onTutorialCompletedCb?.Invoke();
        }
        
        #endif
        

        
        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _onTUtorialCompleted?.Invoke();
        }


    }
}