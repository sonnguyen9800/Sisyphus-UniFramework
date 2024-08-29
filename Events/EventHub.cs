using System;
using System.Collections.Generic;
using UnityCommunity.UnitySingleton;

namespace SisyphusLab
{
    public class EventHub<T1> : Singleton<EventHub<T1>> where T1: System.Enum
    {
        private readonly IDictionary<T1, EventHandler> Events = new Dictionary<T1, EventHandler>();

        public void Subscribe(T1 eventType, EventHandler listener)
        {
            if (!Events.ContainsKey(eventType))
            {
                Events[eventType] = null;
            }
            Events[eventType] += listener;
        }
        /// <summary>
        /// Subscribe only onced, then auto unscribe
        /// </summary>
        /// <typeparam name="T1">The type of event key used to distinguish between different events.</typeparam>
        public void SubscribeOnced(T1 eventType, EventHandler listener)
        {
            EventHandler eventWrapper = null;
            eventWrapper = (sender, args) =>
            {
                listener(sender, args);
                Unsubscribe(eventType, eventWrapper);
            };
            Subscribe(eventType, eventWrapper);
        }
        
        public void Unsubscribe(T1 eventType, EventHandler listener)
        {
            if (Events.ContainsKey(eventType))
            {
                Events[eventType] -= listener;
            }
        }

        public void Publish(T1 eventType)
        {
            if (Events.ContainsKey(eventType) && Events[eventType] != null)
            {
                Events[eventType]?.Invoke(null, EventArgs.Empty);
            }
        }
        public void Publish(T1 eventType, EventArgs param)
        {
            if (Events.ContainsKey(eventType) && Events[eventType] != null)
            {
                Events[eventType]?.Invoke(null, param);
            }
        }
    }

}
