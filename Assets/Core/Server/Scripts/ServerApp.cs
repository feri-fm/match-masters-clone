using System;
using System.Collections.Generic;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Driver;
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
        }

        public void Stop()
        {
            app.Stop();
        }

        private void __Start()
        {
            var connectionString = "mongodb://localhost:27017";
            // var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
            // if (connectionString == null)
            // {
            //     Console.WriteLine("You must set your 'MONGODB_URI' environment variable. To learn how to set it, see https://www.mongodb.com/docs/drivers/csharp/current/quick-start/#set-your-connection-string");
            //     Environment.Exit(0);
            // }
            var client = new MongoClient(connectionString);
            var collection = client.GetDatabase("karo-ludo").GetCollection<BsonDocument>("users");
            var filter = Builders<BsonDocument>.Filter.Eq("username", "User_17");
            var document = collection.Find(filter).First();
            Debug.Log(document);
        }
    }
}