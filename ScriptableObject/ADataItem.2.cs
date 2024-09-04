using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SisyphusLab.Utils
{
    public abstract class ADataItem<T, TEnum>
    where TEnum : System.Enum
    {
        public TEnum Type;
        public T Data;
    }

}