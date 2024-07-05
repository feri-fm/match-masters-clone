using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MMC.Server
{
    public abstract class Module
    {
        public ServerApp app;

        public void Setup(ServerApp app)
        {
            this.app = app;
        }

        public virtual void Build()
        {
            foreach (var fieldInfo in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                var attr = fieldInfo.GetCustomAttribute<UseModuleAttribute>();
                if (attr != null)
                {
                    var module = app.Find(fieldInfo.FieldType);
                    if (module != null)
                    {
                        fieldInfo.SetValue(this, module);
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ModuleAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class UseModuleAttribute : Attribute
    {
    }
}