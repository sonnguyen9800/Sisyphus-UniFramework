using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using SisyphusLab.Data;

namespace SisyphusLab.Floodfill
{
    public static class Floodfill
    {
        public static HashSet<IFloodFill> GetAllNodes(IFloodFill firstNode)
        {
            var allNodes = new HashSet<IFloodFill> { firstNode };
            var potentialNodes = new UniqueStack<IFloodFill>();
            potentialNodes.Push(firstNode);

            ProgressFlooding(allNodes, potentialNodes);
            return allNodes;
        }

        public static HashSet<T> GetAllNodes<T>(T firstNode) where T : IFloodFill
        {
            var allNodes = new HashSet<IFloodFill> { firstNode };
            var potentialNodes = new UniqueStack<IFloodFill>();
            potentialNodes.Push(firstNode);

            ProgressFlooding(allNodes, potentialNodes);
            return allNodes.OfType<T>().ToHashSet();
        }

        private static void ProgressFlooding(
            HashSet<IFloodFill> registeredSet,
            UniqueStack<IFloodFill> potentialSet,
            object param = null)
        {
            while (potentialSet.Count > 0)
            {
                var currentNode = potentialSet.Pop();
                var connectedNodes = currentNode.TryTraverse(param);

                // Only add nodes that are not yet in the registeredSet (i.e., haven't been processed)
                foreach (var node in connectedNodes)
                {
                    if (!registeredSet.Contains(node))
                    {
                        potentialSet.Push(node);
                        registeredSet.Add(node);
                    }
                }
            }
        }
    }

    public interface IFloodFill
    {
        HashSet<IFloodFill> TryTraverse(object param = null);
    }
}