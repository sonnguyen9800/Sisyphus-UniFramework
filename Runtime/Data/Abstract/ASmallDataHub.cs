
using SisyphusFramework.Utils;
using UnityEngine;

namespace SisyphusFramework.Data
{
    /// <summary>
    /// ADataHub for small set of data (the id is byte), max value contained is 255
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ASmallDataHub<T> : ADataHub<T, byte> 
        where T : IPlayData<byte>
    
    {
        protected ASmallDataHub()
        {
            _dataDictionary.Clear();
            _dataArray = new T[] { };
        }


        public abstract override void Init();
        public abstract override void OnEditedSuccess(byte id, T oldData, T newData);
        public abstract override void OnDeletedSuccess(byte id);
        public abstract override void OnAddedSuccess(byte id, T newData);
        


        /// <summary>
        /// Add and return the new Id for created item. Id is assigned automatically
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override byte Add(T data)
        {
            if (data.GetId() != 0)
            {
                Debug.LogError("Invalid data input!. You probaly set ID for the data. " +
                               "This is incorrect. Leaves data ID to 0");
                return 0;
            }

            byte id;
            if (_discardedItemId.Count > 0)
                id = _discardedItemId.Pop();
            else
            {
                id = (byte)(_currentLastestId + 1);
                _currentLastestId += 1;
            }
            data.SetId(id);
            var x=  data.GetId();
            
            _dataDictionary[id] = data;
            RebuildArray();
            OnAddedSuccess(id, data);
            return id;

        }
        
        public override void Add(T[] data)
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

        public override void ShowData()
        {
                        
#if UNITY_EDITOR
            
            var properties = typeof(T).GetFields();
            

            
            GUILayout.BeginHorizontal();
            foreach (var property in properties)
            {
                var name = property.Name;
                GUILayout.Label(property.Name, GUILayout.Width(name.Length * 10));
            }
            GUILayout.EndHorizontal();

            var allData = GetAllData();

            foreach (var data in allData)
            {
                GUILayout.BeginHorizontal();

                foreach (var property in properties)
                {
                    var value = data.GetFieldValue(property.Name);
                    GUILayout.Label(value.ToString(), GUILayout.Width(property.Name.Length * 10));
                }

                GUILayout.EndHorizontal();

            }

#endif
        }
    }

    
    

}


