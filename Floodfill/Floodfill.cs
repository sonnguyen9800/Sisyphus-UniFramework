using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using SisyphusLab.Data;

namespace SisyphusLab.Floodfill
{
    
    public static class Floodfill
    {
        
        public static HashSet<IFloodFill> GetAllNode(IFloodFill firstNode)
        {
            HashSet<IFloodFill> allNodes = new HashSet<IFloodFill>();
            allNodes.Add(firstNode);
            UniqueStack<IFloodFill> potentialBag = new UniqueStack<IFloodFill>();
            potentialBag.Push(firstNode);
            ProgressFlooding(registeredSet: ref allNodes, potentialSet: ref potentialBag);
            return allNodes;
        }
        public static HashSet<T> GetAllNode<T>(IFloodFill firstNode) where T : IFloodFill
        {
            HashSet<IFloodFill> allNodes = new HashSet<IFloodFill>();
            allNodes.Add(firstNode);
            UniqueStack<IFloodFill> potentialBag = new UniqueStack<IFloodFill>();
            potentialBag.Push(firstNode);
            ProgressFlooding(registeredSet: ref allNodes, potentialSet: ref potentialBag);
            return allNodes.Cast<T>().ToHashSet();;
        }
        private static (HashSet<IFloodFill>, UniqueStack<IFloodFill>) ProgressFlooding(
            ref HashSet<IFloodFill> registeredSet, 
            ref UniqueStack<IFloodFill> potentialSet, 
            object param = null)
        {

            // define base-state
            if (potentialSet.Count == 0)
            {
                return (registeredSet, potentialSet);
            }

            var firstState = potentialSet.Pop();
            var allConnected = firstState.TryTraverse(param);
            potentialSet.AddRange(allConnected);
            registeredSet.AddRange(allConnected);
            return ProgressFlooding(ref registeredSet, ref potentialSet);
        }
    }

    public interface IFloodFill
    {
        public HashSet<IFloodFill> TryTraverse(object param = null);
        
    }
}