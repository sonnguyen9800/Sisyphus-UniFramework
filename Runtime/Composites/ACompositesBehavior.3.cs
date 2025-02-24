using System.Collections.Generic;

namespace SisyphusLab.Composites
{
    public abstract class ACompositeBehavior<T, T1> 
        where T : System.Enum
        where T1 : struct
    {
        private readonly Dictionary<T, IBehavior<T1>> _behaviorDct = new();
        private readonly List<IBehavior<T1>> _behaviors = new();

        public virtual void AddBehavior(T key, IBehavior<T1> behavior)
        {
            if (_behaviorDct.TryGetValue(key, out IBehavior<T1> currentBehavior))
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

        public virtual bool RemoveBehavior(IBehavior<T1> behavior)
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

        protected virtual void Execute(T1 param)
        {
            foreach (var behavior in _behaviors)
            {
                behavior.Execute(param);
            }
        }

        #endregion
        
    }
}