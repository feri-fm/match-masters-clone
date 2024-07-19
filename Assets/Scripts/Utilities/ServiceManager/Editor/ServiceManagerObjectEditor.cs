using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ServiceManagerConfig))]
public class ServiceManagerObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var serviceManager = target as ServiceManagerConfig;
        serializedObject.Update();

        var index = serviceManager.groups.FindIndex(e => e.name == serviceManager.selectedGroupNameEditor);
        var selectedGroupIndex = EditorGUILayout.Popup("Editor Group", index, serviceManager.groups.Select(e => e.name).ToArray());
        if (selectedGroupIndex >= 0)
            serviceManager.selectedGroupNameEditor = serviceManager.groups[selectedGroupIndex].name;

        index = serviceManager.groups.FindIndex(e => e.name == serviceManager.selectedGroupNameBuild);
        selectedGroupIndex = EditorGUILayout.Popup("Build Group", index, serviceManager.groups.Select(e => e.name).ToArray());
        if (selectedGroupIndex >= 0)
            serviceManager.selectedGroupNameBuild = serviceManager.groups[selectedGroupIndex].name;

        EditorUtility.SetDirty(target);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("groups"));
        serializedObject.ApplyModifiedProperties();
    }
}
