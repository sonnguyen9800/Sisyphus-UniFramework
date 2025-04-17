using System;
using SisyphusFramework.ScriptableObject;
using SisyphusFramework.Utils;
using UnityEngine;

namespace SisyphusFramework.GUI.Popup
{

    [Serializable]
    public abstract class APopupItem<TPopupName> : AResourcesItem<TPopupName, GameObject>
    where TPopupName : Enum
    {
        
    }
    [CreateAssetMenu(menuName = "SisyphusFramework/Popup Database")]
    public abstract class APopupDatabase<TPopupName, TPopupItem>
        : AScriptableDatabase<TPopupName, TPopupItem, GameObject>,
        IPopupDatabase
    where TPopupName : struct, Enum
    where TPopupItem : APopupItem<TPopupName>
    {
        public GameObject GetPopupByName(string name)
        {
            if (!Enum.TryParse(name, out TPopupName popupName))
                return null;
            var prefab = GetByType(popupName);
            if (prefab == null)
            {
                Debug.LogError($"Popup prefab not found for name: {name}");
                return null;
            }
            return prefab;
        }
    }

    public interface IPopupDatabase
    {
        public GameObject GetPopupByName(string name);
    }
}