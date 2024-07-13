using System;
using System.Collections.Generic;
using Mirror;
using MMC.Game;
using MMC.Network.GameMiddleware;
using MMC.Network.SessionMiddleware;
using MMC.Server;
using MongoDB.Driver.Linq;
using UnityEngine;

namespace MMC.Network
{
    public partial class NetNetworkManager : NetworkManager
    {
        public ServerManager serverManager;
        public ServiceManager serviceManager;
        public List<NetNetworkMiddleware> middlewares = new();

        public SessionNetworkMiddleware session { get; private set; }
        public GameNetworkMiddleware game { get; private set; }

        public GameManager gameManager => GameManager.instance;

        public GameServiceDriver gameService => serviceManager.GetService<GameServiceDriver>();

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

            var transport = GetComponent<TelepathyTransport>();
            transport.port = gameService.transportPort;

            //TODO: start server shouldn't happen on game scene, just for testing
            if (Application.isEditor)
                StartServer();

#if UNITY_SERVER
            StartServer();
#endif
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
            var transport = GetComponent<TelepathyTransport>();
            transport.port = connection.port;
            StartClient();
        }

        public async void Kick(NetworkConnectionToClient conn, string message)
        {
            conn.Send(new KickClientMessage(message));
            await new WaitForSeconds(0.2f);
            conn.Disconnect();
        }

        public override void OnStartClient()
        {
            base.OnStartClient(); ForEachMiddleware(e => e.OnStartClient());
            NetworkClient.RegisterHandler<KickClientMessage>(OnKickClientMessage);
        }
        public override void OnStopClient()
        {
            base.OnStopClient(); ForEachMiddleware(e => e.OnStopClient());
            NetworkClient.UnregisterHandler<KickClientMessage>();
        }
        public override void OnClientConnect() { base.OnClientConnect(); ForEachMiddleware(e => e.OnClientConnect()); }
        public override void OnClientDisconnect() { base.OnClientDisconnect(); ForEachMiddleware(e => e.OnClientDisconnect()); }

        public override void OnStartServer() { base.OnStartServer(); ForEachMiddleware(e => e.OnStartServer()); }
        public override void OnStopServer() { base.OnStopServer(); ForEachMiddleware(e => e.OnStopServer()); }
        public override void OnServerConnect(NetworkConnectionToClient conn) { base.OnServerConnect(conn); ForEachMiddleware(e => e.OnServerConnect(conn)); }
        public override void OnServerDisconnect(NetworkConnectionToClient conn) { ForEachMiddleware(e => e.OnServerDisconnect(conn)); base.OnServerDisconnect(conn); }


        public void OnKickClientMessage(KickClientMessage msg)
        {
            Popup.ShowAlert(msg.message);
            gameManager.isConnected = false;
        }
    }

    public class NetworkConnectionInfo
    {
        public string address;
        public ushort port;
    }

    public struct KickClientMessage : NetworkMessage
    {
        public string message;

        public KickClientMessage(string message)
        {
            this.message = message;
        }
    }
}