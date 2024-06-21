using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MemberBinder : MonoBehaviour
{
    public bool bindOnAwake = true;
    public MonoBehaviour target;

    public const string METHOD_MEMBER_PREFIX = "M_";

    private bool hasBind;

    private void Awake()
    {
        if (bindOnAwake)
            Bind();
    }

    public void Bind()
    {
        if (hasBind) return;
        var methods = target.GetType().GetMethods();
        foreach (var method in methods)
        {
            var attrs = method.GetCustomAttributes(typeof(MemberAttribute), true);
            var names = new List<string>();
            foreach (var atr in attrs)
            {
                var attr = atr as MemberAttribute;
                if (attr != null)
                {
                    var n = attr.GetName(method.Name);
                    names.Add(n);
                }
            }
            if (names.Count == 0 && method.Name.StartsWith(METHOD_MEMBER_PREFIX))
            {
                names.Add(method.Name.Substring(METHOD_MEMBER_PREFIX.Length));
            }
            foreach (var obj in GetAllChildren(transform))
            {
                foreach (var btnName in names)
                {
                    if (obj.name == btnName)
                    {
                        var btn = obj.GetComponent<ButtonHelper>();
                        if (btn != null)
                        {
                            btn.onClick.AddListener(() =>
                            {
                                method.Invoke(target, new object[] { });
                            });
                        }
                    }
                }
            }
        }

        var memberFields = target.GetType().GetFields()
            .Where(e => typeof(Member).IsAssignableFrom(e.FieldType))
            .ToArray();
        var members = memberFields
            .Select(e => e.GetValue(target) as Member)
            .ToArray();

        for (int i = 0; i < memberFields.Length; i++)
        {
            if (members[i] == null)
            {
                var member = (Member)Activator.CreateInstance(memberFields[i].FieldType);
                memberFields[i].SetValue(target, member);
                members[i] = member;
            }

            var attrs = memberFields[i].GetCustomAttributes(typeof(MemberAttribute), true);
            members[i].names = attrs.Select(e => e as MemberAttribute)
                .Where(e => e != null).Select(e => e.GetName(memberFields[i].Name)).ToList();
            if (members[i].names.Count == 0)
                members[i].names.Add(MemberAttribute.GetLabelName(memberFields[i].Name));
        }

        foreach (var obj in GetAllChildren(transform))
        {
            foreach (var member in members)
            {
                if (member.names.Contains(obj.name))
                {
                    member.Register(obj);
                }
            }
        }
        hasBind = true;
    }

    public static IEnumerable<GameObject> GetAllChildren(Transform transform)
    {
        var parents = new List<Transform>();
        parents.Add(transform);

        while (parents.Count > 0)
        {
            var parent = parents[0];
            for (int i = 0; i < parent.childCount; i++)
            {
                var item = parent.GetChild(i);
                if (item.GetComponent<MemberBinder>() == null)
                {
                    parents.Add(item);
                }
                yield return item.gameObject;
            }
            parents.RemoveAt(0);
        }
    }

    private void Reset()
    {
        FetchTarget();
    }

    public void FetchTarget()
    {
        foreach (var comp in GetComponents<MonoBehaviour>())
        {
            if (comp.GetType().GetMembers().Any(e
                => (e.MemberType == MemberTypes.Field
                        && typeof(Member).IsAssignableFrom((e as FieldInfo).FieldType))
                    || e.GetCustomAttributes(typeof(MemberAttribute), true).Length > 0))
            {
                target = comp;
                return;
            }
        }
    }

    public static void Bind(MonoBehaviour target)
    {
        target.GetComponent<MemberBinder>().Bind();
    }
}


public class Member
{
    public List<string> names;

    public virtual void Register(GameObject obj) { }
    public virtual bool CanRegister(GameObject obj) => false;
}

public class Member<T> : Member
{
    public List<T> comps = new List<T>();

    public T value => comps.Count > 0 ? comps[0] : default(T);

    public void For(UnityAction<T> action)
    {
        foreach (var comp in comps)
        {
            action.Invoke(comp);
        }
    }

    public override void Register(GameObject obj)
    {
        base.Register(obj);
        var comp = obj.GetComponent<T>();
        if (comp != null) comps.Add(comp);
    }
    public override bool CanRegister(GameObject obj)
    {
        return obj.GetComponent<T>() != null;
    }
}
public class GameObjectMember : Member
{
    public List<GameObject> gameObjects = new List<GameObject>();

    public bool activeSelf => gameObjects[0].activeSelf;

    public GameObject first => gameObjects[0];

    public override void Register(GameObject obj)
    {
        base.Register(obj);
        gameObjects.Add(obj);
    }
    public override bool CanRegister(GameObject obj) => true;

    public void SetActive(bool value)
    {
        foreach (var obj in gameObjects)
        {
            obj.SetActive(value);
        }
    }

    public void For(UnityAction<GameObject> action)
    {
        foreach (var comp in gameObjects)
        {
            action.Invoke(comp);
        }
    }
}

public class TextMember : Member<TextHelper>
{
    private string _text;

    public string text
    {
        get => _text;
        set => SetText(value);
    }

    public void SetText(string text)
    {
        _text = text;
        For(e => e.text = text);
    }
}

public class ListLoaderMember : Member<ListLoader>
{
    public void Setup(IEnumerable items)
    {
        For(e => e.Setup(items));
    }

    public void Clear()
    {
        For(e => e.Clear());
    }
}

public class ImageMember : Member<Image>
{
    public Sprite sprite
    {
        get => value.sprite;
        set => For(e => e.sprite = value);
    }

    public Color color
    {
        get => value.color;
        set => For(e => e.color = value);
    }
}

[System.AttributeUsage(System.AttributeTargets.All, Inherited = true, AllowMultiple = true)]
public sealed class MemberAttribute : System.Attribute
{
    public string name { get; }

    public bool noName;

    public MemberAttribute()
    {
        noName = true;
    }
    public MemberAttribute(string name)
    {
        noName = false;
        this.name = name;
    }

    public string GetName(string def)
    {
        if (noName) return def[0].ToString().ToUpper() + def.Substring(1);
        return name;
    }
    public static string GetLabelName(string val)
    {
        return val[0].ToString().ToUpper() + val.Substring(1);
    }
}
