using System;
using System.Collections.Generic;

namespace SisyphusLab.Data
{
    // TODO: improve this
    public class UniqueStack<T>
    {
        private HashSet<T> _set = new HashSet<T>();
        private Stack<T> _stack = new Stack<T>();

        public bool Push(T item)
        {
            if (_set.Add(item))
            {
                _stack.Push(item);
                return true;
            }
            return false; // Item already exists in the stack
        }

        public T Pop()
        {
            if (_stack.Count > 0)
            {
                T item = _stack.Pop();
                _set.Remove(item);
                return item;
            }
            throw new InvalidOperationException("The stack is empty.");
        }

        public T Peek()
        {
            if (_stack.Count > 0)
            {
                return _stack.Peek();
            }
            throw new InvalidOperationException("The stack is empty.");
        }

        public int Count => _stack.Count;

        public bool Contains(T item) => _set.Contains(item);

        public void Clear()
        {
            _stack.Clear();
            _set.Clear();
        }

        public void AddRange(IEnumerable<T> collected)
        {
            foreach (var item in collected)
            {
                Push(item);
            }
        }
        
    }
}