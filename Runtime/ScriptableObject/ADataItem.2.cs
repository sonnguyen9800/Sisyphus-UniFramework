﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SisyphusFramework.Utils
{
    public abstract class AResourcesItem<TEnum,T>
    where TEnum : System.Enum
    {
        public TEnum Type;
        public T Data;
    }

}