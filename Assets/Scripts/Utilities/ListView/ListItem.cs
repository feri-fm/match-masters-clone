using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListItem : PoolObject
{
    public object data { get; private set; }
    public int index { get; private set; }
    public ListLoader loader { get; private set; }

    public void _Setup(ListLoader loader, object data, int index)
    {
        this.loader = loader;
        this.data = data;
        this.index = index;
        Setup();
    }

    public virtual void Setup() { }
}

public class ListItem<T> : ListItem
{
    public new T data => (T)base.data;
}
