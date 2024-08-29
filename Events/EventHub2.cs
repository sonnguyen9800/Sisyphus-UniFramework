using System;
using System.Collections.Generic;
using UnityCommunity.UnitySingleton;

namespace SisyphusLab
{

    public class EventHub< T1, T2> : Singleton<EventHub<T1, T2>> 
        where T1 : Enum
        where T2 : EventArgs 
    {
        public delegate void SEventHandler(object sender, T2 e);

        private readonly IDictionary<T1, SEventHandler> Events = new Dictionary<T1, SEventHandler>();

        public void Subscribe(T1 eventType, SEventHandler listener)
        {
            if (!Events.ContainsKey(eventType))
            {
                Events[eventType] = null;
            }

            Events[eventType] += listener;
        }

        public void Unsubscribe(T1 eventType, SEventHandler listener)
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