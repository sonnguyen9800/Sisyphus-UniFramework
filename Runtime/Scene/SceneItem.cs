using System;
using UnityEditor;
using UnityEngine;

namespace SisyphusFramework.ScriptableObject.Scene
{
    [Serializable]
    public class SceneItem
    {
        [SerializeField] public String ScenePath;
#if UNITY_EDITOR
        [SerializeField] public SceneAsset Scene;


#endif
        private void UpdateScenePath()
        {
#if UNITY_EDITOR

            if (Scene != null)
            {
                ScenePath = AssetDatabase.GetAssetPath(Scene);
            }
#endif

        }
        // This method is called whenever values are changed in the Inspector
        private void OnValidate()
        {
            UpdateScenePath();
        }

    }

}

