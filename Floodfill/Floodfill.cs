using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using SisyphusLab.Data;

namespace SisyphusLab.Floodfill
{
    
    public static class Floodfill
    {
        public static int Count = 0;
        public static HashSet<IFloodFill> GetAllNode(IFloodFill firstNode)
        {
            HashSet<IFloodFill> allNodes = new HashSet<IFloodFill>();
            allNodes.Add(firstNode);
            UniqueStack<IFloodFill> potentialBag = new UniqueStack<IFloodFill>();
            potentialBag.Push(firstNode);
            HashSet<IFloodFill> removedSet = new();

            ProgressFlooding(registeredSet: ref allNodes, potentialSet: ref potentialBag, removedSet: ref removedSet);
            return allNodes;
        }
        public static HashSet<T> GetAllNode<T>(IFloodFill firstNode) where T : IFloodFill
        {
            Count = 0;
            HashSet<IFloodFill> allNodes = new() { firstNode };
            UniqueStack<IFloodFill> potentialBag = new();
            potentialBag.Push(firstNode);
            HashSet<IFloodFill> removedSet = new();
            ProgressFlooding(
                registeredSet: ref allNodes, 
                potentialSet: ref potentialBag, 
                removedSet: ref removedSet);
            return allNodes.Cast<T>().ToHashSet();;
        }
        private static (HashSet<IFloodFill>, UniqueStack<IFloodFill>) ProgressFlooding(
            ref HashSet<IFloodFill> registeredSet, 
            ref UniqueStack<IFloodFill> potentialSet, 
            ref HashSet<IFloodFill> removedSet,

        object param = null)
        {

            // define base-state
            if (potentialSet.Count == 0 || Count == 100)
            {
                return (registeredSet, potentialSet);
            }

            var firstState = potentialSet.Pop();
            var allConnected = firstState.TryTraverse(param);
            removedSet.Add(firstState);
            foreach (var item in removedSet)
            {
                allConnected.Remove(item);
            }
            
            potentialSet.AddRange(allConnected);
            registeredSet.AddRange(allConnected);
            Count++;
            
            return ProgressFlooding(ref registeredSet, ref potentialSet, ref removedSet);
        }
    }

    public interface IFloodFill
    {
        public HashSet<IFloodFill> TryTraverse(object param = null);
        
    }
}