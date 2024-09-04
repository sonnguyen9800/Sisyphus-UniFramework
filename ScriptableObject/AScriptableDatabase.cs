using System.Collections.Generic;
using SisyphusLab.Utils;
using UnityEngine;

namespace SisyphusLab.ScriptableObject
{
    public abstract class AScriptableDatabase<T, T1> : UnityEngine.ScriptableObject where T : ADataItem<T1>
    {
        [SerializeField] protected List<T> _data;

        Dictionary<int, T1> _dataDict = new();

        private void OnEnable()
        {
            _dataDict.Clear();
            foreach (var item in _data)
            {
                if (_dataDict.ContainsKey(item.Id))
                    continue;
                _dataDict.Add(item.Id, item.Data);
            }
        }



        public T1 GetById(int id)
        {
            return _dataDict[id];
        }
    }

}
