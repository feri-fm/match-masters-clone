using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Events;

public class PanelGroup : Panel
{
    public Transform root;
    public string[] startPanels = new string[0];

    public List<Panel> panels { get; } = new List<Panel>();

    public Panel topPanel { get; private set; } = null;

    private bool startSetup = false;
    private bool awakeSetup = false;

    public event UnityAction<Panel> onPanelOpen = (p) => { };
    public event UnityAction<Panel> onPanelClose = (p) => { };

    public override void Awake()
    {
        if (root == null) root = transform;
        base.Awake();
        DoAwakeSetup();
    }
    public override void Start()
    {
        base.Start();
        DoStartSetup();
        for (int i = 0; i < startPanels.Length; i++)
            OpenPanel(startPanels[i]);
    }

    public void DoAwakeSetup()
    {
        if (awakeSetup) return;
        awakeSetup = true;
        panels.Clear();
        for (int i = 0; i < root.childCount; i++)
        {
            var obj = root.GetChild(i);
            var panel = obj.GetComponent<Panel>();
            if (panel != null)
                panels.Add(panel);
        }
        for (int i = 0; i < panels.Count; i++)
            panels[i].Init(this, i);
    }

    private void DoStartSetup()
    {
        if (startSetup) return;
        startSetup = true;
        for (int i = 0; i < panels.Count; i++)
            panels[i]._Setup();
        _Setup();
    }

    public override void Init(PanelGroup group, int index)
    {
        base.Init(group, index);
        DoAwakeSetup();
    }

    public override void Setup()
    {
        base.Setup();
        DoStartSetup();
    }

    public Panel GetPanel(string key)
    {
        for (int i = 0; i < panels.Count; i++)
            if (panels[i].key == key)
                return panels[i];
        return null;
    }
    public Panel GetPanel(int index)
    {
        return panels[index];
    }
    public T GetPanel<T>() where T : Panel
    {
        for (int i = 0; i < panels.Count; i++)
            if (panels[i] is T)
                return panels[i] as T;
        return null;
        // return GetPanel(GetPanelName<T>()) as T;
    }

    public void OpenPanel(string key)
    {
        OpenPanel(GetPanel(key));
    }
    public void OpenPanel(Panel panel)
    {
        panel.transform.SetAsLastSibling();
        panel._SetOpen(true);
        if (panel.stack)
        {
            RecalculateTopPanel();
        }
        else
        {
            foreach (var p in panels)
            {
                if (p.isOpen && p != panel)
                {
                    p._SetOpen(false);
                    p._OnClose();
                }
            }
            topPanel = panel;
        }
        panel._OnOpen();
        onPanelOpen.Invoke(panel);
    }
    public void ClosePanel(Panel panel)
    {
        panel._SetOpen(false);
        RecalculateTopPanel();
        panel._OnClose();
        onPanelClose.Invoke(panel);
    }

    public void RecalculateTopPanel()
    {
        topPanel = null;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var obj = transform.GetChild(i);
            var panel = obj.GetComponent<Panel>();
            if (panel != null && panel.isOpen)
            {
                topPanel = panel;
                break;
            }
        }
    }
}
