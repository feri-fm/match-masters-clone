using System.Collections.Generic;

namespace MMC.EngineCore
{
    public delegate void EventAction(EventParams evt);

    public class EventManger
    {
        public Dictionary<string, EventListener> eventListeners { get; } = new Dictionary<string, EventListener>();

        public void Listen(object key, string eventName, EventAction action)
        {
            if (!eventListeners.ContainsKey(eventName))
                eventListeners.Add(eventName, new EventListener(eventName));
            eventListeners[eventName].Add(key, action);
        }
        public void Remove(object key, string eventName)
        {
            if (eventListeners.ContainsKey(eventName))
                eventListeners[eventName].Remove(key);
        }
        public void RemoveKey(object key)
        {
            foreach (var e in eventListeners) e.Value.Remove(key);
        }
        public void Call(string eventName) => Call(eventName, null);
        public void Call(string eventName, Entity sender, params object[] args)
        {
            if (!eventListeners.ContainsKey(eventName)) return;
            var eventListener = eventListeners[eventName];
            eventListener.Call(sender, args);
        }

        public void Clear()
        {
            eventListeners.Clear();
        }
    }

    public class EventParams
    {
        public string eventName;
        public Entity sender;
        public object[] args;
    }
    public class EventListener
    {
        public string eventName;
        public Dictionary<object, EventAction> actions = new Dictionary<object, EventAction>();

        public EventListener(string eventName)
        {
            this.eventName = eventName;
        }

        public void Add(object key, EventAction action)
        {
            actions[key] = action;
        }
        public void Remove(object key)
        {
            actions.Remove(key);
        }

        public void Call(Entity sender, params object[] args)
        {
            foreach (var action in actions.Values)
                action.Invoke(new EventParams()
                {
                    sender = sender,
                    args = args,
                    eventName = eventName
                });
        }
    }
}