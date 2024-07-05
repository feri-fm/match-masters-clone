using System;
using System.Collections.Generic;
using kcp2k;
using Mirror;
using MMC.Network.Game;
using MMC.Network.Session;
using MMC.Server;
using MongoDB.Driver.Linq;
using UnityEngine;

namespace MMC.Network
{
    public partial class NetNetworkManager : NetworkManager
    {
        public ServerManager serverManager;
        public List<NetNetworkMiddleware> middlewares = new();

        public SessionNetworkMiddleware session { get; private set; }
        public GameNetworkMiddleware game { get; private set; }

        public static NetNetworkManager instance { get; private set; }

        public override void Awake()
        {
            base.Awake();
            instance = this;

            session = middlewares.Find(e => e is SessionNetworkMiddleware) as SessionNetworkMiddleware;
            game = middlewares.Find(e => e is GameNetworkMiddleware) as GameNetworkMiddleware;
        }

        public override void Start()
        {
            base.Start();
            ForEachMiddleware(e => e._Setup(this));

            //TODO: start server shouldn't happen on game scene, just for testing
            StartServer();
        }

        public void ForEachMiddleware(Action<NetNetworkMiddleware> action)
        {
            foreach (var middleware in middlewares)
            {
                action.Invoke(middleware);
            }
        }

        public string GetStatus()
        {
            if (NetworkServer.active && NetworkClient.active)
                return $"<b>Host</b>: running via {Transport.active}";
            else if (NetworkServer.active)
                return $"<b>Server</b>: running via {Transport.active}";
            else if (NetworkClient.isConnected)
                return $"<b>Client</b>: connected to {networkAddress} via {Transport.active}";
            else if (NetworkClient.active)
                return $"<b>Client</b>: connecting to {networkAddress}...";
            return "Not connected";
        }

        public void StartClient(NetworkConnectionInfo connection)
        {
            networkAddress = connection.address;
            var transport = GetComponent<KcpTransport>();
            transport.port = connection.port;
            StartClient();
        }

        public override void OnStartClient() { base.OnStartClient(); ForEachMiddleware(e => e.OnStartClient()); }
        public override void OnStopClient() { base.OnStopClient(); ForEachMiddleware(e => e.OnStopClient()); }
        public override void OnClientConnect() { base.OnClientConnect(); ForEachMiddleware(e => e.OnClientConnect()); }
        public override void OnClientDisconnect() { base.OnClientDisconnect(); ForEachMiddleware(e => e.OnClientDisconnect()); }

        public override void OnStartServer() { base.OnStartServer(); ForEachMiddleware(e => e.OnStartServer()); }
        public override void OnStopServer() { base.OnStopServer(); ForEachMiddleware(e => e.OnStopServer()); }
        public override void OnServerConnect(NetworkConnectionToClient conn) { base.OnServerConnect(conn); ForEachMiddleware(e => e.OnServerConnect(conn)); }
        public override void OnServerDisconnect(NetworkConnectionToClient conn) { ForEachMiddleware(e => e.OnServerDisconnect(conn)); base.OnServerDisconnect(conn); }
    }

    public class NetworkConnectionInfo
    {
        public string address;
        public ushort port;
    }
}