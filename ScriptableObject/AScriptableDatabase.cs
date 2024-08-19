
using System.Collections.Generic;
using UnityEngine;


namespace SisyphusLab.Utils
{
    public abstract class AScriptableDatabase<T, T1> : ScriptableObject where T : ADataItem<T1>
    {
        [SerializeField] protected List<T> _data;

        Dictionary<int, T1> _dataDict = new Dictionary<int, T1>();

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
