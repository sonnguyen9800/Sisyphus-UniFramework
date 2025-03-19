using System.Collections.Generic;

namespace SisyphusFramework.Data
{
    public static class VisualizerRegistry
    {
        private static readonly List<IVisualizer> _visualizers = new List<IVisualizer>();
    
        public static void Register(IVisualizer visualizer)
        {
            if (!_visualizers.Contains(visualizer))
            {
                _visualizers.Add(visualizer);
            }
        }
    
        public static void Unregister(IVisualizer visualizer)
        {
            if (_visualizers.Contains(visualizer))
            {
                _visualizers.Remove(visualizer);
            }
        }
    
        public static IVisualizer[] GetAllVisualizers()
        {
            return _visualizers.ToArray();
        }
    }
}