using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SisyphusLab.Utils
{
    public abstract class ADataItem<T>
    {
        public int Id;
        public T Data;
    }

}
