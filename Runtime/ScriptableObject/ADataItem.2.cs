using System;

namespace SisyphusFramework.Utils
{
    [Serializable]
    public abstract class AResourcesItem<TEnum,T>
    where TEnum : System.Enum
    {
        public TEnum Type;
        public T Data;
    }

}