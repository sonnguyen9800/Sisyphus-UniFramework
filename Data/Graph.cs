using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SisyphusLab.Data
{
    public class Graph<T> : ICollection<T>
    {
        private readonly Dictionary<T, List<T>> adjacencyList = new();

        private void AddVertex(T vertex)
        {
            if (!adjacencyList.ContainsKey(vertex))
            {
                adjacencyList[vertex] = new List<T>();
            }
        }

        public void AddLeaf(T parent, T leaf)
        {
            AddVertex(parent);
            AddVertex(leaf);
            adjacencyList[parent].Add(leaf);
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

        /// <summary>
        /// Recursively extract an item and all sequential children (adjancency) into a HashSet
        /// </summary>
        /// <param name="item"></param>
        /// <param name="extractedSet">All items that had been extracted (recursive)</param>
        public void Extract(T item, HashSet<T> extractedSet)
        {
            if (!adjacencyList.ContainsKey(item))
                return;
            extractedSet.Add(item);
            
            // Remove all edges referencing the removed vertex
            foreach (var neighbor in adjacencyList[item])
            {
                Extract(neighbor, extractedSet);
            }
            adjacencyList.Remove(item);

            return;
        }
        public IEnumerator<T> GetEnumerator() => adjacencyList.Keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
