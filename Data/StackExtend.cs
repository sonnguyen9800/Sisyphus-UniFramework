using System;
using System.Collections.Generic;

namespace SisyphusLab.Data
{
    public static class UtilsData
    {
        public static void DequeueUntilOneLeft<T>(this Stack<T> stack)
        {
                while (stack.Count > 1)
                {
                    stack.Pop();
                }
        }

    }
}