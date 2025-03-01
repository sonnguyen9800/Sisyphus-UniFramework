using System.Collections.Generic;
using SisyphusFramework.Utils;
using UnityEngine;

namespace SisyphusFramework.ScriptableObject
{
    /// <summary>
    /// Scriptable Database with Enum for indexing
    /// </summary>
    /// <typeparam name="TEnum">TEnum must be Serializeable</typeparam>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T1"></typeparam>
    public abstract class AScriptableDatabase<TEnum,T, T1> : UnityEngine.ScriptableObject
        where TEnum : System.Enum 
        where T : AResourcesItem<TEnum,T1 >

    {
        [SerializeField] protected List<T> _data;

        Dictionary<TEnum, T1> _dataDict = new();

        private void OnEnable()
        {
            hideFlags = HideFlags.DontUnloadUnusedAsset; // Prevents Unity from unloading this ScriptableObject
            _dataDict.Clear();

            if (_data == null) return; // Ensure _data is not null

            foreach (var item in _data)
            {
                if (item != null && !_dataDict.ContainsKey(item.Type))
                {
                    _dataDict[item.Type] = item.Data;
                }
            }
        }



        public T1 GetByType(TEnum type)
        {
            return _dataDict[type];
        }
    }

}