using System;
using System.Collections;
using System.Collections.Generic;

namespace SisyphusLab.Data
{
    public class Graph<T> : ICollection<T>
    {
        private Dictionary<T, List<T>> adjacencyList;

        public Graph()
        {
            adjacencyList = new Dictionary<T, List<T>>();
        }

        public void AddVertex(T vertex)
        {
            if (!adjacencyList.ContainsKey(vertex))
            {
                adjacencyList[vertex] = new List<T>();
            }
        }

        public void AddEdge(T source, T destination)
        {
            AddVertex(source);
            AddVertex(destination);
            adjacencyList[source].Add(destination);
        }

        public List<T> GetNeighbors(T vertex)
        {
            return adjacencyList.ContainsKey(vertex) ? adjacencyList[vertex] : new List<T>();
        }

        // ICollection<T> Implementation
        public int Count => adjacencyList.Count;

        public bool IsReadOnly => false;

        public void Add(T item) => AddVertex(item);
        
        
  

        public bool Contains(T item) => adjacencyList.ContainsKey(item);

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count) throw new ArgumentException("Insufficient space in target array.");

            foreach (var vertex in adjacencyList.Keys)
            {
                array[arrayIndex++] = vertex;
            }
        }
        public void Clear()
        {
            adjacencyList.Clear();
        }
        public bool Remove(T item)
        {
            if (!adjacencyList.ContainsKey(item)) return false;

            adjacencyList.Remove(item);

            // Remove all edges referencing the removed vertex
            foreach (var neighbors in adjacencyList.Values)
            {
                neighbors.Remove(item);
            }

            return true;
        }

        public IEnumerator<T> GetEnumerator() => adjacencyList.Keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
