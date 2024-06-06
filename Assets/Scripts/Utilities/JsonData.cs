
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

public class JsonData
{
    public JObject json { get; }

    public JsonData()
    {
        json = new JObject();
    }
    public JsonData(JObject json)
    {
        this.json = json ?? new JObject();
    }

    public void W(string key, object data)
    {
        json[key] = JToken.FromObject(data);
    }
    public T R<T>(string key, T defaultValue)
    {
        if (json.ContainsKey(key))
            return json[key].ToObject<T>();
        return defaultValue;
    }

    private static Dictionary<Type, FieldInfo[]> dataLoaderFields = new Dictionary<Type, FieldInfo[]>();

    public static FieldInfo[] GetDataLoaderFields(Type type)
    {
        if (dataLoaderFields.TryGetValue(type, out var fields))
            return fields;
        fields = type.GetFields().Where(e => Attribute.IsDefined(e, typeof(JsonDataAttribute))).ToArray();
        dataLoaderFields[type] = fields;
        return fields;
    }
    public void Save(object target) => Save(target, this);
    public static void Save(object target, JsonData data)
    {
        var fields = GetDataLoaderFields(target.GetType());
        foreach (var field in fields)
        {
            var value = field.GetValue(target);
            var attr = field.GetCustomAttributes<JsonDataAttribute>().First();
            data.W(attr.key, value);
        }
    }

    public void Load(object target) => Load(target, this);
    public static void Load(object target, JsonData data)
    {
        var fields = GetDataLoaderFields(target.GetType());
        foreach (var field in fields)
        {
            var attr = field.GetCustomAttributes<JsonDataAttribute>().First();
            if (data.json.TryGetValue(attr.key, out var raw))
                field.SetValue(target, attr.loader.Invoke(raw));
            else
                field.SetValue(target, attr.defaultValue);
        }
    }
}

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public abstract class JsonDataAttribute : Attribute
{
    public string key { get; }
    public object defaultValue { get; }
    public Func<JToken, object> loader { get; protected set; }

    public JsonDataAttribute(string key, object defaultValue, Func<JToken, object> loader)
    {
        this.key = key;
        this.defaultValue = defaultValue;
        this.loader = loader;
    }
}

public class JsonDataIntAttribute : JsonDataAttribute
{
    public JsonDataIntAttribute(string key, int defaultValue = 0)
        : base(key, defaultValue, (s) => int.Parse(s.ToString())) { }
}
public class JsonDataStringAttribute : JsonDataAttribute
{
    public JsonDataStringAttribute(string key, string defaultValue = "")
        : base(key, defaultValue, (s) => s.ToString()) { }
}
