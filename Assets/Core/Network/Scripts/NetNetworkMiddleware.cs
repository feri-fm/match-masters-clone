using System;
using System.Collections.Generic;
using Mirror;
using MMC.Game;
using MMC.Network.SessionMiddleware;
using MMC.Server;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace MMC.Network
{
    public abstract class NetNetworkMiddleware : MonoBehaviour
    {
        public NetNetworkManager manager { get; private set; }

        public NetNetworkMiddlewareClient client;
        public NetNetworkMiddlewareServer server;

        public void _Setup(NetNetworkManager manager)
        {
            this.manager = manager;
            client = CreateClient();
            server = CreateServer();
            client._Setup(this);
            server._Setup(this);
        }

        public abstract NetNetworkMiddlewareClient CreateClient();
        public abstract NetNetworkMiddlewareServer CreateServer();

        private void Update()
        {
            server.Update();
            client.Update();
        }
        private void LateUpdate()
        {
            server.LateUpdate();
            client.LateUpdate();
        }

        public void OnStartClient() { client.OnStart(); }
        public void OnStopClient() { client.OnStop(); }
        public void OnClientConnect() { client.OnConnect(); }
        public void OnClientDisconnect() { client.OnDisconnect(); }

        public void OnStartServer() { server.OnStart(); }
        public void OnStopServer() { server.OnStop(); }
        public void OnServerConnect(NetworkConnectionToClient conn) { server.OnConnect(conn); }
        public void OnServerDisconnect(NetworkConnectionToClient conn) { server.OnDisconnect(conn); }
    }


    public class ServerListener
    {
        public string key;
        public List<Action<Session, JToken>> actions = new();

        public ServerListener(string key)
        {
            this.key = key;
        }

        public void Invoke(Session session, JToken data)
        {
            foreach (var action in actions)
            {
                action.Invoke(session, data);
            }
        }
    }

    public class ClientListener
    {
        public string key;
        public List<Action<JToken>> actions = new();

        public ClientListener(string key)
        {
            this.key = key;
        }

        public void Invoke(JToken data)
        {
            foreach (var action in actions)
            {
                action.Invoke(data);
            }
        }
    }

    public abstract class NetNetworkMiddleware<TServer, TClient> : NetNetworkMiddleware
        where TServer : NetNetworkMiddlewareServer
        where TClient : NetNetworkMiddlewareClient
    {
        public new TServer server => base.server as TServer;
        public new TClient client => base.client as TClient;
    }
}