using System;
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
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ModuleAttribute : Attribute
    {
    }
}