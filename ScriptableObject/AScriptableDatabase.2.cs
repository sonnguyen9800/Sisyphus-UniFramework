using System.Collections.Generic;
using UnityEngine;


namespace SisyphusLab.Utils
{
    public abstract class AScriptableDatabase<T, TEnum, T1> : ScriptableObject
        where TEnum : System.Enum 
        where T : ADataItem<T1, TEnum>

    {
        [SerializeField] protected List<T> _data;

        Dictionary<TEnum, T1> _dataDict = new();

        private void OnEnable()
        {
            _dataDict.Clear();
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