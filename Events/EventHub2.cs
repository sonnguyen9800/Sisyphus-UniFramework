using System;
using System.Collections.Generic;
using UnityCommunity.UnitySingleton;

namespace SisyphusLab
{

    public class EventHub< T1, T2> : Singleton<EventHub<T1, T2>> 
        where T1 : Enum
        where T2 : struct 
    {
        public delegate void SisyphusEventHandler(object sender, T2 e);

        private readonly IDictionary<T1, SisyphusEventHandler> Events = new Dictionary<T1, SisyphusEventHandler>();

        public void Subscribe(T1 eventType, SisyphusEventHandler listener)
        {
            if (!Events.ContainsKey(eventType))
            {
                Events[eventType] = null;
            }

            Events[eventType] += listener;
        }
        public void SubscribeOnced(T1 eventType, SisyphusEventHandler listener)
        {
            SisyphusEventHandler eventWrapper = null;
            eventWrapper = (sender, args) =>
            {
                listener(sender, args);
                Unsubscribe(eventType, eventWrapper);
            };
            Subscribe(eventType, eventWrapper);
        }
        public void Unsubscribe(T1 eventType, SisyphusEventHandler listener)
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
                Events[eventType]?.Invoke(null, default(T2));
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