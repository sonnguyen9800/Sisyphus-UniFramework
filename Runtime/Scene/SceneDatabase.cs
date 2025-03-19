using System;
using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace SisyphusFramework.ScriptableObject.Scene
{
    [Serializable]
    public class SceneItemResource : AScriptableItem<SceneItem, string>
    {
        public override string Indexing => Data.ScenePath;

        public void UpdateScenePath()
        {
            #if UNITY_EDITOR
            if (Data.Scene != null)
            {
                string fullPath = AssetDatabase.GetAssetPath(Data.Scene); // e.g., "Assets/Scenes/MyScene.unity"
                Data.ScenePath = Path.ChangeExtension(fullPath, null).Replace("Assets/", ""); // "Scenes/MyScene"
            }
            #endif
        }
    }
    [CreateAssetMenu(menuName = "FAW/Scene Database")]
    public class SceneDatabase : ASODatabase<SceneItemResource, SceneItem, string> {
        private void OnValidate()
        {
            foreach (var data in _data)
            {
                data.UpdateScenePath();
            }
        }

        public int Count => _data?.Count() ?? 0;

        public string[] GetAllItemPath()
        {
            return _data.Select(a => a.Data.ScenePath).ToArray();
        }
    }
}