using UnityEngine;

public abstract class ListItem : PoolObject
{
    public ListLoader loader { get; private set; }

    public object data { get; private set; }

    protected virtual void Setup()
    {
    }

    public virtual bool IsEqual(object data)
    {
        return data == this.data;
    }

    public void _SetupLoader(ListLoader loader)
    {
        this.loader = loader;
    }

    public void _SetupData(object data)
    {
        this.data = data;
        Setup();
    }
}

public class ListItem<T> : ListItem where T : class
{
    public new T data => base.data as T;

    public sealed override bool IsEqual(object data)
    {
        return IsEqual(data as T);
    }

    protected virtual bool IsEqual(T data)
    {
        return base.IsEqual(data);
    }
}