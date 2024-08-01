using FAW;
using FAW.ScritableObject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SisyphusLab.Utils
{
    public abstract class AScriptableDatabase<T, T1> : ScriptableObject where T : ADataItem<T1>
    {
        [SerializeField] List<T> _data;

        Dictionary<string, T1> _dataDict = new Dictionary<string, T1>();

        private void Awake()
        {
            _data = new List<T>();
            foreach (var item in _data)
            {
                if (_dataDict.ContainsKey(item.Id))
                    continue;
                _dataDict.Add(item.Id, item.Data);
            }
        }


    }

}
