using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using UnityEngine;
using WebServer;

namespace MMC.Server
{
    public class ServerManager : MonoBehaviour
    {
        public ServerApp app;

        private void Start()
        {
            Setup();
        }

        public void Setup()
        {
            app = new ServerApp();
            app.RegisterAssemblyModules();
            app.Build();
            app.Listen(3000);
        }

        private void OnDestroy()
        {
            app.Stop();
        }
    }
}