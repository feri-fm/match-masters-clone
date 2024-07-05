using System;
using System.Collections.Generic;
using JWT.Algorithms;
using JWT.Builder;
using MongoDB.Bson;
using MongoDB.Driver;
using UnityEngine;
using WebServer;

namespace MMC.Server
{
    public class ServerManager : MonoBehaviour
    {
        public ushort port = 3000;

        public ServerApp app;

        private void Awake()
        {
            Setup();
        }

        private void Start()
        {
            //TODO: start server shouldn't happen on game scene, just for testing
            StartServer();
        }

        private void Setup()
        {
            app = new ServerApp();
            app.RegisterAssemblyModules();
            app.Build();
        }

        public void StartServer()
        {
            app.Listen(port);
        }
        public void StopServer()
        {
            app.Stop();
        }

        private void OnDestroy()
        {
            StopServer();
        }
    }
}