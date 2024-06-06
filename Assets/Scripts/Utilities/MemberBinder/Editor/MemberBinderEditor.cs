using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

[CustomEditor(typeof(MemberBinder), true)]
public class MemberBinderEditor : ScriptlessEditor
{
    public MemberData[] members;

    private MonoBehaviour lastTarget;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var target = base.target as MemberBinder;


        if (target.target == null)
        {
            EditorGUILayout.HelpBox("Target is empty", MessageType.Warning);
            if (GUILayout.Button("Find Target"))
            {
                Undo.RecordObject(target, "Find target");
                target.FetchTarget();
                PrefabUtility.RecordPrefabInstancePropertyModifications(target);
            }
            return;
        }

        if (members == null || lastTarget != target.target)
        {
            Build();
            lastTarget = target;
        }

        GUILayout.BeginVertical("Members", "window");
        var added = false;
        foreach (var member in members)
        {
            foreach (var attr in member.attributesNames)
            {
                added = true;
                var b = member.bindings.Count;
                GUILayout.Label($"<color=#ffffff><b>{attr} <color=#{(b > 0 ? "00ffff" : "ff0000")}>{b}</color></b> ({member.memberName}: {member.memberType})</color>", new GUIStyle()
                {
                    richText = true,
                });
            }
        }
        if (!added)
        {
            EditorGUILayout.HelpBox("No member found", MessageType.Info);
        }

        if (GUILayout.Button("Update"))
        {
            Build();
        }
        GUILayout.EndVertical();
    }

    public void Build()
    {
        var res = new List<MemberData>();

        var binder = base.target as MemberBinder;

        var members = binder.target.GetType().GetMembers()
            .Where(e => (e.MemberType == MemberTypes.Field
                        && typeof(Member).IsAssignableFrom((e as FieldInfo).FieldType))
                    || e.GetCustomAttributes(typeof(MemberAttribute), true).Length > 0
                    || e.MemberType == MemberTypes.Method)
            .ToList();
        var membersAttrs = members
            .Select(e => e.GetCustomAttributes(typeof(MemberAttribute), true))
            .ToList();
        for (int i = 0; i < members.Count; i++)
        {
            var type = "???";
            Member member = null;
            if (members[i].MemberType == MemberTypes.Field)
            {
                var m = members[i] as FieldInfo;
                type = m.FieldType.ToString();
                member = m.GetValue(binder.target) as Member;
                if (member == null)
                {
                    member = (Member)Activator.CreateInstance(m.FieldType);
                    m.SetValue(binder.target, member);
                }
            }
            else if (members[i].MemberType == MemberTypes.Method)
            {
                var m = members[i] as MethodInfo;
                type = "Method";
            }
            var data = new MemberData()
            {
                memberInfo = members[i],
                member = member,
                memberType = type,
                memberName = members[i].Name,
                bindings = new List<GameObject>(),
                attributesNames = membersAttrs[i]
                    .Select(e => e as MemberAttribute)
                    .Select(e => e.GetName(members[i].Name))
                    .ToList(),
            };
            if (data.attributesNames.Count == 0)
            {
                if (members[i].MemberType == MemberTypes.Method)
                {
                    if (members[i].Name.StartsWith(MemberBinder.METHOD_MEMBER_PREFIX))
                    {
                        data.attributesNames.Add(MemberAttribute.GetLabelName(members[i].Name.Substring(MemberBinder.METHOD_MEMBER_PREFIX.Length)));
                    }
                }
                else
                {
                    data.attributesNames.Add(MemberAttribute.GetLabelName(members[i].Name));
                }
            }

            res.Add(data);
        }

        foreach (var obj in MemberBinder.GetAllChildren(binder.target.transform))
        {
            foreach (var mem in res)
            {
                if (mem.attributesNames.Contains(obj.name))
                {
                    if (mem.memberInfo.MemberType == MemberTypes.Method)
                    {
                        if (obj.GetComponent<ButtonHelper>() != null)
                            mem.bindings.Add(obj);
                    }
                    else if (mem.memberInfo.MemberType == MemberTypes.Field)
                    {
                        if (mem.member.CanRegister(obj))
                            mem.bindings.Add(obj);
                    }
                }
            }
        }

        this.members = res.ToArray();
    }

    public class MemberData
    {
        public MemberInfo memberInfo;
        public Member member;
        public string memberType;
        public string memberName;
        public List<GameObject> bindings;
        public List<string> attributesNames;
    }
}