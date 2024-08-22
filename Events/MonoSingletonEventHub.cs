using System;
using System.Collections.Generic;
using UnityCommunity.UnitySingleton;

namespace SisyphusLab
{

    public class MonoSingletonEventHub<T1, T2> : MonoSingleton<MonoSingletonEventHub<T1, T2>>
        where T2 : EventArgs 
        where T1 : System.Enum
    {
        public delegate void FAWEventHandler(object sender, T2 e);

        private readonly IDictionary<T1, FAWEventHandler> Events = new Dictionary<T1, FAWEventHandler>();

        public void Subscribe(T1 eventType, FAWEventHandler listener)
        {
            if (!Events.ContainsKey(eventType))
            {
                Events[eventType] = null;
            }

            Events[eventType] += listener;
        }

        public void Unsubscribe(T1 eventType, FAWEventHandler listener)
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

        public void Publish(T1 eventType, T2 param)
        {
            if (Events.ContainsKey(eventType) && Events[eventType] != null)
            {
                Events[eventType]?.Invoke(null, param);
            }
        }
    }
}