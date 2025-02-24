﻿using System;
using System.Collections.Generic;
using UnityCommunity.UnitySingleton;

namespace SisyphusLab
{
    /// <summary>
    /// This EventHub only applied for Singleton
    /// 
    /// </summary>
    /// <typeparam name="T">The class of Hub</typeparam>
    /// <typeparam name="T1">Signal Enum (enum)</typeparam>
    /// <typeparam name="T2">Extra Parameter (struct)</typeparam>
    public class EventHub<T, T1, T2> : Singleton<T>
        where T: Singleton<T>,  new()
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