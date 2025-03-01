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
            _dataDict.Clear();
            _data = new List<T>();

            foreach (var item in _data)
            {
                if (_dataDict.ContainsKey(item.Type))
                    continue;
                _dataDict.Add(item.Type, item.Data);
            }
        }



        public T1 GetByType(TEnum type)
        {
            return _dataDict[type];
        }
    }

}