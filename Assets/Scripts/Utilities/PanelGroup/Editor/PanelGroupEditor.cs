using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(PanelGroup), true)]
public class PanelGroupEditor : Editor
{
    public Panel[] panels;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var target = base.target as PanelGroup;

        if (panels == null)
        {
            Build();
        }

        GUILayout.BeginVertical("Panels", "window");
        foreach (var panel in panels)
        {
            GUILayout.Label($"{panel.key}");
        }
        if (panels.Length == 0)
            GUILayout.Label("There's no panel!");
        GUILayout.EndVertical();
    }

    public void Build()
    {
        var target = base.target as PanelGroup;
        var root = target.root;
        if (root == null) root = target.transform;
        var panels = new List<Panel>();
        for (int i = 0; i < root.childCount; i++)
        {
            var obj = root.GetChild(i);
            var panel = obj.GetComponent<Panel>();
            if (panel != null)
                panels.Add(panel);
        }
        this.panels = panels.ToArray();
    }
}