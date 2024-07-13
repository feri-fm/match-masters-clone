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
        public ServerApp app;

        public GameServiceDriver gameService => ServiceManager.instance.GetService<GameServiceDriver>();

        private void Awake()
        {
            Setup();
        }

        private void Start()
        {
            //TODO: start server shouldn't happen on game scene, just for testing
            if (Application.isEditor)
                StartServer();

#if UNITY_SERVER
            StartServer();
#endif
        }

        private void Setup()
        {
            app = new ServerApp();
            app.RegisterAssemblyModules();
            app.Build();
        }

        public void StartServer()
        {
            app.Listen(gameService.serverPort);
        }
        public void StopServer()
        {
            app.Stop();
        }

        private void OnDestroy()
        {
            StopServer();
        }
        private void OnApplicationQuit()
        {
            StopServer();
        }
    }
}