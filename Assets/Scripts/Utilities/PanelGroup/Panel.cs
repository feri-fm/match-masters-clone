using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Panel : MonoBehaviour
{
    public string key;
    public bool stack;

    public PanelGroup group { get; private set; }
    public int index { get; private set; }

    public bool isOpen { get; private set; }
    public bool isTop => group.topPanel == this;

    public event UnityAction onOpen = () => { };
    public event UnityAction onClose = () => { };

    private bool didSetup = false;

    public static string GetPanelName(GameObject go, System.Type type) => type.Name == "Panel" ? (go?.name ?? "Panel") : type.Name.Substring(0, type.Name.Length - 5);
    public static string GetPanelName<T>() where T : Panel => GetPanelName(null, typeof(T));

    public virtual void Setup() { }
    public virtual void OnOpen() { }
    public virtual void OnClose() { }

    public virtual void Awake() { }
    public virtual void Start() { }
    public virtual void Update() { }
    public virtual void LateUpdate() { }
    public virtual void FixedUpdate() { }

    public virtual void Init(PanelGroup group, int index)
    {
        this.group = group;
        this.index = index;
        _SetOpen(false);
    }
    public void _Setup()
    {
        if (didSetup) return;
        didSetup = true;
        Setup();
    }

    public void _OnOpen() { OnOpen(); onOpen?.Invoke(); }
    public void _OnClose() { OnClose(); onClose?.Invoke(); }

    public void _SetOpen(bool value)
    {
        isOpen = value;
        gameObject.SetActive(value);
    }

    public void OpenPanel() => group.OpenPanel(this);
    public void ClosePanel() => group.ClosePanel(this);

    private void Reset()
    {
        key = GetPanelName(gameObject, GetType());
    }
}
