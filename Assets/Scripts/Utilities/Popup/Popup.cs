using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Popup : PoolObject
{
    public string key => name;

    public PopupOptions options { get; private set; }

    public void Init()
    {
        SetActive(false);
    }

    public virtual void Setup() { }

    public void _Setup(PopupOptions options)
    {
        this.options = options;
        Setup();
    }

    public void Close()
    {
        Pool();
    }

    public static AlertPopup ShowAlert(string text) => PopupManager.instance.ShowAlert(text) as AlertPopup;
    public static ConfirmationPopup ShowConfirmation(string text, UnityAction action) => PopupManager.instance.ShowConfirmation(text, action) as ConfirmationPopup;
}
public class Popup<T> : Popup
{
    public T data;

    public override void Setup()
    {
        base.Setup();
        data = (T)options.data;
    }
}

public class PopupOptions
{
    public object data;
}
