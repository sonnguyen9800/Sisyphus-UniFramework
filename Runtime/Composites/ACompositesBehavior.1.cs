using System.Collections.Generic;

namespace SisyphusFramework.Composites
{
    public abstract class ACompositeBehavior<T> where T : System.Enum
    {
        private readonly Dictionary<T, IBehavior> _behaviorDct = new();
        private readonly List<IBehavior> _behaviors = new();

        public virtual void AddBehavior(T key, IBehavior behavior)
        {
            if (_behaviorDct.TryGetValue(key, out IBehavior currentBehavior))
            {
                //do nothing
                return;
            }

            _behaviorDct[key] = behavior;
            _behaviors.Add(behavior);
        }

        public virtual void ClearBehaviors()
        {
            _behaviors.Clear();
        }

        public virtual bool RemoveBehavior(IBehavior behavior)
        {
            _behaviors.Remove(behavior);
            T key = default(T);
            foreach (var iter in _behaviorDct)
            {
                if (iter.Value == behavior)
                {
                    key = iter.Key;
                }
            }
            return _behaviorDct.Remove(key);
        }

        public virtual bool RemoveBehavior(T key)
        {
            if (!_behaviorDct.TryGetValue(key, out var behavior))
                return false;
            _behaviors.Remove(behavior);
            return _behaviorDct.Remove(key);
        }

        #region Main

        protected virtual void Execute(object param = null)
        {
            foreach (var behavior in _behaviors)
            {
                behavior.Execute(param);
            }
        }

        #endregion
        
    }
}