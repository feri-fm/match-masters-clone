using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using WebServer;

namespace MMC.Server
{
    public class ServerApp
    {
        public WebServerApp app;
        public Router router;

        public List<Module> modules = new();

        public ServerApp()
        {
            app = new WebServerApp();
            router = new Router("/api");
            app.Setup(router);
        }

        public void AddModule(Module module)
        {
            module.Setup(this);
            modules.Add(module);
        }

        public T Find<T>() where T : Module
        {
            return modules.Find(e => e is T) as T;
        }
        public Module Find(Type type)
        {
            return modules.Find(e => type.IsAssignableFrom(e.GetType()));
        }

        public void RegisterAssemblyModules()
        {
            foreach (var type in Assembly.GetAssembly(GetType()).GetTypes())
            {
                var attr = type.GetCustomAttribute(typeof(ModuleAttribute), true);
                if (attr != null)
                {
                    var module = Activator.CreateInstance(type) as Module;
                    AddModule(module);
                }
            }
        }

        public void Build()
        {
            foreach (var module in modules)
            {
                module.Build();
            }

            router.Use(async (req, res) =>
            {
                await res.Send(new
                {
                    name = "this is the not found page",
                    Time.time,
                    check = true,
                    req.httpRequest.HttpMethod,
                    req.httpRequest.Url,
                    req.httpRequest.UrlReferrer,
                    req.httpRequest.RawUrl,
                    req.httpRequest.QueryString,
                });
            });

            router.Use(new ExceptionHandler());

            router.Build();
        }

        public void Listen(ushort port)
        {
            app.Listen(port);
            Debug.Log("Server listening on " + port);
        }

        public void Stop()
        {
            app.Stop();
            Debug.Log("Server stopped");
        }
    }
}