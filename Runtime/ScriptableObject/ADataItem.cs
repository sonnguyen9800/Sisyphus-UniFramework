using System;
namespace SisyphusFramework.Utils
{
    [Serializable]
    public abstract class ADataItem<T>
    {
        public int Id;
        public T Data;
    }

}
