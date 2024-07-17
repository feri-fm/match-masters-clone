using UnityEngine;

public abstract class ListItem : PoolObject
{
    public ListLoader loader { get; private set; }

    public int index { get; private set; }
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

    public void _Setup(int index, object data)
    {
        this.index = index;
        this.data = data;
        Setup();
    }
}

public class ListItem<T> : ListItem
{
    public new T data => (T)base.data;

    public sealed override bool IsEqual(object data)
    {
        return IsEqual((T)data);
    }

    protected virtual bool IsEqual(T data)
    {
        return base.IsEqual(data);
    }
}