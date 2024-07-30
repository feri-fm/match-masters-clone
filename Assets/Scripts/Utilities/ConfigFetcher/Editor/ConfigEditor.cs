using System;
using System.Reflection;
using UnityEditor;

namespace ConfigFetcher
{
    public class ConfigEditor
    {
        [MenuItem("Config Fetcher/Fetch All")]
        public static void FetchAll()
        {
            foreach (var type in Assembly.GetAssembly(typeof(ConfigEditor)).GetTypes())
            {
                var attr = type.GetCustomAttribute(typeof(ConfigFetchAttribute), true) as ConfigFetchAttribute;
                if (attr != null)
                {
                    var targetType = attr.type;
                    var fetch = Activator.CreateInstance(type) as ConfigFetch;
                    foreach (var guid in AssetDatabase.FindAssets($"t:{targetType}"))
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        var asset = AssetDatabase.LoadAssetAtPath(path, targetType);
                        fetch.Fetch(asset);
                    }
                }
            }
        }
    }

    public abstract class ConfigFetch
    {
        public abstract void Fetch(UnityEngine.Object target);
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigFetchAttribute : Attribute
    {
        public Type type;

        public ConfigFetchAttribute(Type type)
        {
            this.type = type;
        }
    }
}