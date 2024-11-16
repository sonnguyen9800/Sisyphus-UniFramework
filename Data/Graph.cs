using System;
using System.Collections.Generic;

namespace SisyphusLab.Data
{
    public class Graph<T>
    {
        private readonly Dictionary<T, List<T>> adjacencyList = new();

        public void AddVertex(T vertex)
        {
            if (!adjacencyList.ContainsKey(vertex))
            {
                adjacencyList[vertex] = new List<T>();
            }
        }

        public void AddEdge(T source, T destination)
        {
            if (!adjacencyList.ContainsKey(source))
            {
                AddVertex(source);
            }
            if (!adjacencyList.ContainsKey(destination))
            {
                AddVertex(destination);
            }
            adjacencyList[source].Add(destination);
        }

        public List<T> GetNeighbors(T vertex)
        {
            return adjacencyList.ContainsKey(vertex) ? adjacencyList[vertex] : new List<T>();
        }
    }
}
