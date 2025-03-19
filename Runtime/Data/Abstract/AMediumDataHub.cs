using SisyphusFramework.Utils;
using UnityEngine;

namespace SisyphusFramework.Data
{
    /// <summary>
    /// Medium Size DataHub (Id type is ushort) Maxium size is 65k
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AMediumDataHub<T> : ADataHub<T, ushort> 
        where T : IPlayData<ushort>
    {
        public override ushort Add(T data)
        {
            if (data.GetId() != 0)
            {
                Debug.LogError("Invalid data input!. You probaly set ID for the data. " +
                               "This is incorrect. Leaves data ID to 0");
                return 0;
            }

            ushort id;
            if (_discardedItemId.Count > 0)
                id = _discardedItemId.Pop();
            else
            {
                id = (ushort)(_currentLastestId + 1);
                _currentLastestId += 1;
            }
            data.SetId(id);
            var x=  data.GetId();
            
            _dataDictionary[id] = data;
            RebuildArray();
            OnAddedSuccess(id, data);
            return id;
        }
        

        private Vector2 verticalScrollPos = Vector2.zero;
        private Vector2 horizontalContentScrollPos = Vector2.zero;

        public override void ShowData()
        {
                        
#if UNITY_EDITOR
            var properties = typeof(T).GetFields();
            
            
            /// Fields
            GUILayout.BeginHorizontal();
            foreach (var property in properties)
            {
                var name = property.Name;
                GUILayout.Label(property.Name, GUILayout.Width(name.Length * 10 + 5));
            }
            GUILayout.EndHorizontal();

            /// Datas

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