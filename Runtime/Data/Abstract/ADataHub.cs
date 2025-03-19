using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using Object = System.Object;

namespace SisyphusFramework.Data
{

    /// <summary>
    /// Use this ADataHub to customize size of id
    /// </summary>
    /// <typeparam name="T">IPlayModel</typeparam>
    /// <typeparam name="T1">The type for ID (should be numeric)</typeparam>
    public abstract class ADataHub<T,T1> : IVisualizer 
        where T : IPlayData<T1> 
        where T1 : struct
    {
        protected readonly Dictionary<T1, T> _dataDictionary = new();
        protected T[] _dataArray;
        protected readonly Stack<T1> _discardedItemId = new();
        protected readonly HashSet<T1> _manuallyAddedIds = new();
        protected T1 _currentLastestId;

        protected ADataHub()
        {
            _dataDictionary.Clear();
            _dataArray = new T[] { };
        }

        public abstract void Init();
        public abstract void OnEditedSuccess(T1 id, T oldData, T newData);
        public abstract void OnDeletedSuccess(T1 id);
        public abstract void OnAddedSuccess(T1 id, T newData);

        public T[] GetAllData()
        {
            return _dataArray;
        }

        public void ClearAllData()
        {
            _dataDictionary.Clear();
            _manuallyAddedIds.Clear();
            _discardedItemId.Clear();
            RebuildArray();
        }
        public T1[] GetAllDataId()
        {
            return _dataDictionary.Keys.ToArray();
        }

        public Dictionary<T1, T> GetAllDataByReference()
        {
            return _dataDictionary;
        }

        protected void RebuildArray()
        {
            //TODO: this is inefficient! Improve later
            _dataArray = new List<T>(_dataDictionary.Values).ToArray();
        }


        /// <summary>
        /// Add and return the new Id for created item
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract T1 Add(T data);

        public bool Add(T data, T1 id)
        {
            if (_dataDictionary.TryGetValue(id, out _))
            {
                Debug.LogError("Data Existed!");
                return false;
            }
            data.SetId(id);
            _dataDictionary[id] = data;
            _manuallyAddedIds.Add(id);
            RebuildArray();
            OnAddedSuccess(id, data);
            return true;
        }
        public virtual void Add(T[] data)
        {
            foreach (var item in data)
            {
                if (_dataDictionary.ContainsKey(item.GetId()))
                {
                    Debug.LogError("Key already exist: $" + item.GetId());
                }
                else
                {
                    _dataDictionary[item.GetId()] = item;
                    RebuildArray();
                }
            }

        }

        public void Remove(T1[] ids)
        {
            foreach (var id in ids)
            {
                Remove(id);
            }
        }
        public void Remove(T1 id)
        {
            if (_dataDictionary.ContainsKey(id))
            {
                _dataDictionary.Remove(id);

                if (_manuallyAddedIds.Contains(id))
                    _manuallyAddedIds.Remove(id);
                else
                    _discardedItemId.Push(id);
                
                RebuildArray();
                OnDeletedSuccess(id: id);

            }
            else
            {
                Debug.LogError("Key not found: $" + id);
            }

        }

        public virtual bool Override(T1 id, T newData)
        {
            if (!_dataDictionary.ContainsKey(id))
            {
                Debug.LogError("Key not found: $" + id);
                return false;
            }

            var oldData = _dataDictionary[id];
            _dataDictionary[id] = newData;
            RebuildArray();
            OnEditedSuccess(id, oldData, newData);
            return true;
        }
        
        
        public bool Edit(T1 id, Dictionary<Expression<Func<T, object>>, object> updates)
        {
            if (!_dataDictionary.TryGetValue(id, out var oldData))
            {
                Debug.LogError("Key not found: $" + id);
                return false;
            }
            Object newData = oldData;

            foreach (var update in updates)
            {
                var propertyExpression = update.Key;
                var newValue = update.Value;
                string name = string.Empty;
                if (propertyExpression.Body is MemberExpression memberExpression)
                {
                    name = memberExpression.Member.Name;
                }
                else if (propertyExpression.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression unaryMemberExpression)
                {
                    name = unaryMemberExpression.Member.Name;
                }
                
                var fieldInfo = typeof(T).GetField(name);
                if (fieldInfo == null)
                {
                    Debug.LogError("Null property expression");
                    return false;
                }
                if (!fieldInfo.IsPublic)
                {
                    Debug.LogError("Property is not public (read-only)");
                    return false;
                }
                try
                {
                    fieldInfo.SetValue(newData, newValue);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Failed to set property: $" + ex.Message);
                    return false;
                }
            }
            
            _dataDictionary[id] = (T)newData;
            RebuildArray();
            OnEditedSuccess(id, oldData, _dataDictionary[id]);
            return true;
        }
        
        public bool Edit<TProperty>(T1 id, Expression<Func<T, TProperty>> propertyExpression, TProperty newValue)
        {
            if (!_dataDictionary.TryGetValue(id, out var oldData))
            {
                Debug.LogError("Key not found: $" + id);
                return false;
            }

            if (typeof(T).IsValueType && !typeof(T).IsClass)
            {
                var memberExpression = propertyExpression.Body as MemberExpression;
                if (memberExpression == null)
                {
                    Debug.LogError("Invalid property expression");
                    return false;
                }

                var propertyMember = memberExpression.Member;
                var name = propertyMember.Name;
                var fieldInfo = typeof(T).GetField(name);
                if (fieldInfo == null)
                {
                    Debug.LogError("Null property expression");
                    return false;
                }
                if (!fieldInfo.IsPublic)
                {
                    Debug.LogError("Property is not public (read-only)");
                    return false;
                }
                try
                {
                    Object obj = oldData;
                    fieldInfo.SetValue(obj, newValue);
                    _dataDictionary[id] = (T)obj;
                    RebuildArray();
                    OnEditedSuccess(id, oldData, (T)obj);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.LogError("Failed to set property: $" + ex.Message);
                    return false;
                }
                
            }

            Debug.LogError("Data type is Class");
            return false;

        }
        
        
        public (T, bool) GetData(T1 Id)
        {
            if (_dataDictionary.ContainsKey(Id)) return (_dataDictionary[Id], true);
            Debug.LogError("Key not found: $" + Id);
            return (default, false);
        }

        public (T[], bool) GetDatas(T1[] Id)
        {
            List<T> result = new();
            if (Id.Length == 0)
            {
                Debug.LogError("Empty array");
                return (default, false);
            }

            foreach (var id in Id)
            {
                if (_dataDictionary.ContainsKey(id))
                {
                    result.Add(_dataDictionary[id]);
                }
                else
                {
                    Debug.LogError("Key not found: $" + id);
                }
            }

            return (result.ToArray(), true);
        }



        public abstract void ShowData();


    }

    
    

}


