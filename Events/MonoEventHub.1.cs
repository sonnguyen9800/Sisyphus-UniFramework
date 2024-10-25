using System;
using System.Collections.Generic;
using UnityCommunity.UnitySingleton;

namespace SisyphusLab
{
    /// <summary>
    /// MonoSingleton
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T1">Event Name (Enum)</typeparam>
    public class MonoEventHub<T, T1> : MonoSingleton<T> 
        where T : MonoSingleton<T>
        where T1 : Enum
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
                Events[eventType]?.Invoke(null, null);
            }
        }
    }
}