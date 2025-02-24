using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SisyphusFramework.Data
{
    public class Graph<T> : ICollection<T>
    {
        private readonly Dictionary<T, HashSet<T>> adjacencyList = new();

        private void AddVertex(T vertex)
        {
            if (!adjacencyList.ContainsKey(vertex))
            {
                adjacencyList[vertex] = new HashSet<T>();
            }
        }

        public void AddLeaf(T parent, T leaf)
        {
            
            AddVertex(parent);
            AddVertex(leaf);
            adjacencyList[parent].Add(leaf);
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

            // Remove all edges referencing the removed vertex
            foreach (var neighbors in adjacencyList.Values)
            {
                neighbors.Remove(item);
            }

            return  adjacencyList.Remove(item);;
        }

        /// <summary>
        /// Recursively extract an item and all sequential children (adjancency) into a HashSet
        /// </summary>
        /// <param name="item"></param>
        /// <param name="extractedSet">All items that had been extracted (recursive)</param>
        private void Extract(T item, HashSet<T> extractedSet)
        {
            if (!adjacencyList.ContainsKey(item))
                return;
            extractedSet.Add(item);
            // Remove all edges referencing the removed vertex
            
            foreach (var neighbor in adjacencyList[item])
            {
                Extract(neighbor, extractedSet);
            }
        }

        public HashSet<T> Extract(T item)
        {
           var extractedSet = new HashSet<T>();
           Extract(item, extractedSet);
           Remove(extractedSet: extractedSet);
           return extractedSet;
        }
        
        private void Remove(HashSet<T> extractedSet)
        {
            foreach (var item in extractedSet)
            {
                adjacencyList.Remove(item);
            }

            foreach (var adjList in adjacencyList.Values)
            {
                adjList.RemoveWhere(e => extractedSet.Contains(e));
            }
        }
        public IEnumerator<T> GetEnumerator() => adjacencyList.Keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
