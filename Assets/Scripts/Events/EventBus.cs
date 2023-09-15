using System;
using System.Collections.Generic;


namespace UFB.Events
{
    public static class EventBus
    {

        private static Dictionary<Type, object> _eventDictionary = new Dictionary<Type, object>();

        public static void Subscribe<T>(Action<T> handler)
        {
            Type eventType = typeof(T);
            if (!_eventDictionary.ContainsKey(eventType))
            {
                _eventDictionary[eventType] = new List<Action<T>>();
            }
            (_eventDictionary[eventType] as List<Action<T>>).Add(handler);
        }


        public static void Unsubscribe<T>(Action<T> handler)
        {
            Type eventType = typeof(T);
            if (_eventDictionary.ContainsKey(eventType))
            {
                (_eventDictionary[eventType] as List<Action<T>>).Remove(handler);
            }
        }

        public static void Publish<T>(T eventInstance)
        {
            Type eventType = typeof(T);
            if (_eventDictionary.ContainsKey(eventType))
            {
                foreach (var handler in _eventDictionary[eventType] as List<Action<T>>)
                {
                    handler?.Invoke(eventInstance);
                }
            }
        }

        // public static event Action<string, object> OnEvent;

        // public static void Publish(string eventName, object eventArgs)
        // {
        //     OnEvent?.Invoke(eventName, eventArgs);
        // }

        // public static void Subscribe(Action<string, object> action)
        // {
        //     OnEvent += action;
        // }

        // public static void Unsubscribe(Action<string, object> action)
        // {
        //     OnEvent -= action;
        // }
    }

}
