using System;
using System.Collections.Generic;
using UnityEngine;

namespace SisyphusFramework.ScriptableObject
{
    [Serializable]
    public abstract class AScriptableItem<T, T1> where T1: class
    {
        [SerializeField]
        public T Data;
        [HideInInspector]
        public virtual T1 Indexing { get; }
    }
    
    /// <summary>
    /// ```csharp
    /// <summary>
    /// Abstract base class for a ScriptableObject database. The index will be custom-typed
    /// </summary>
    public abstract class ASODatabase<T, Tdata, Tindex> : UnityEngine.ScriptableObject 
        where Tindex: class
        where T : AScriptableItem<Tdata, Tindex>
    {
        // Dictionary<int, T1> _dataDict = new();
        [SerializeField] protected T[] _data;

        private Dictionary<Tindex, Tdata> _dictItem = new();
        private void OnEnable()
        {
            _dictItem = new Dictionary<Tindex, Tdata>();
            if (_data == null)return;
            foreach (var data in _data)
            {
                _dictItem[data.Indexing] = data.Data;
            }
        }

    }
}