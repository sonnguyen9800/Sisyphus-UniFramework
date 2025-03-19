using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SisyphusFramework.Tutorial
{
    [CreateAssetMenu(menuName = "Tutorial/Step")]
    public class TutorialStep : UnityEngine.ScriptableObject
    {
        [SerializeField]
        private string _stepName = "";
        
        public List<ATutorialAction> actions;
        public float delayBeforeNextStep = 0f;
        [HideInInspector] public string Key;
        
        
        
        #if UNITY_WEBGL
        public async UniTask Execute(CancellationToken token)  
        {
            Debug.Log($"Execute step {_stepName}");
            var allTask = new List<UniTask>();
            string allStepName = string.Join(", ", actions.Select(a => a.name));
            Debug.Log($"Start step {allStepName} with {actions.Count} actions :");
            foreach (var action in actions)
            {
                if (token.IsCancellationRequested)
                    return;
                allTask.Add(action.Execute());
            }

            await UniTask.WhenAll(allTask);
            Debug.Log($"Finish step {_stepName}, proceed to handle task on Finished");
            allTask.Clear();
            foreach (var action in actions)
            {
                if (token.IsCancellationRequested)
                    return;
                allTask.Add(action.OnFinished());
            }

            await UniTask.WhenAll(allTask);
            await UniTask.Delay(TimeSpan.FromSeconds(delayBeforeNextStep));

        }
        #else
          public async Task Execute(CancellationToken token)  
        {
            Debug.Log($"Execute step {_stepName}");
            var allTask = new List<Task>();
            string allStepName = string.Join(", ", actions.Select(a => a.name));
            Debug.Log($"Start step {allStepName} with {actions.Count} actions :");
            foreach (var action in actions)
            {
                if (token.IsCancellationRequested)
                    return;
                allTask.Add(action.Execute());
            }

            await Task.WhenAll(allTask);
            Debug.Log($"Finish step {_stepName}, proceed to handle task on Finished");
            allTask.Clear();
            foreach (var action in actions)
            {
                   if (token.IsCancellationRequested)
                return;
                allTask.Add(action.OnFinished());
            }

            await Task.WhenAll(allTask);
            await Task.Delay(TimeSpan.FromSeconds(delayBeforeNextStep));

        }
        #endif
      
    }}