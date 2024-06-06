using System;
using System.Collections.Generic;

public class Evaluator
{
    public List<short> orders { get; } = new List<short>();


    public void Evaluate(IEnumerable<IEvaluable> targets)
    {
        for (int i = 0; i < orders.Count; i++)
        {
            var order = orders[i];
            foreach (var item in targets)
            {
                item.evaluable.Evaluate(order);
            }
        }
    }
}

public interface IEvaluable
{
    public Evaluable evaluable { get; }
}

public class Evaluable
{
    private Dictionary<short, EventHandler> callbacks = new();
    private Evaluator evaluator;

    public Evaluable(Evaluator evaluator)
    {
        this.evaluator = evaluator;
    }

    public void Evaluate(short order)
    {
        if (callbacks.TryGetValue(order, out var eventHandler))
        {
            eventHandler.Invoke();
        }
    }

    public void RegisterCallback(short order, Action callback)
    {
        if (!callbacks.ContainsKey(order))
        {
            callbacks[order] = new EventHandler(order);
        }
        callbacks[order].AddCallback(callback);
        if (!evaluator.orders.Contains(order))
        {
            evaluator.orders.Add(order);
        }
        evaluator.orders.Sort();
    }
    public void RemoveCallback(short order, Action callback)
    {
        if (callbacks.TryGetValue(order, out var eventHandler))
        {
            eventHandler.RemoveCallback(callback);
        }
    }

    private class EventHandler
    {
        public List<Action> callbacks = new();
        public short order;

        public EventHandler(short order)
        {
            this.order = order;
        }

        public void AddCallback(Action action)
        {
            callbacks.Add(action);
        }
        public void RemoveCallback(Action action)
        {
            callbacks.Remove(action);
        }

        public void Invoke()
        {
            foreach (var callback in callbacks)
            {
                callback.Invoke();
            }
        }
    }
}
