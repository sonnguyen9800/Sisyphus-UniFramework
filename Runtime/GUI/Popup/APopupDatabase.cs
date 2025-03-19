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
    public abstract class APopupDatabase<TPopupName, TPopupItem> : AScriptableDatabase<TPopupName, TPopupItem, GameObject>
    where TPopupName : Enum
    where TPopupItem : APopupItem<TPopupName>
    {
    
        
    }
}