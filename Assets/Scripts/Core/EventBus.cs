using System;
using System.Collections.Generic;

public static class EventBus
{
    public static event Action<string, object> OnEvent;

    public static void Publish(string eventName, object eventArgs)
    {
        OnEvent?.Invoke(eventName, eventArgs);
    }

    public static void Subscribe(Action<string, object> action)
    {
        OnEvent += action;
    }

    public static void Unsubscribe(Action<string, object> action)
    {
        OnEvent -= action;
    }
}
