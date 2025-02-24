using System.Collections.Generic;

namespace SisyphusFramework.Composites
{
    public abstract class ACompositeBehavior
    {
        private readonly List<IBehavior> _behaviors = new();

        public virtual void AddBehavior(IBehavior behavior)
        {
            _behaviors.Add(behavior);
        }

        public virtual void ClearBehaviors()
        {
            _behaviors.Clear();
        }

        public virtual bool RemoveBehavior(IBehavior behavior)
        {
            return _behaviors.Remove(behavior);
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