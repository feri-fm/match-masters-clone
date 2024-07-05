using Mirror;
using MMC.Game;
using MMC.Server;
using UnityEngine;

namespace MMC.Network
{
    public class NetNetworkMiddleware : MonoBehaviour
    {
        public NetNetworkManager manager { get; private set; }
        public GameManager gameManager => GameManager.instance;

        public ServerManager serverManager => manager.serverManager;

        public void _Setup(NetNetworkManager manager)
        {
            this.manager = manager;
            SetupServer();
            SetupClient();
        }

        public virtual void SetupClient() { }
        public virtual void OnStartClient() { }
        public virtual void OnStopClient() { }
        public virtual void OnClientConnect() { }
        public virtual void OnClientDisconnect() { }

        public virtual void SetupServer() { }
        public virtual void OnStartServer() { }
        public virtual void OnStopServer() { }
        public virtual void OnServerConnect(NetworkConnectionToClient conn) { }
        public virtual void OnServerDisconnect(NetworkConnectionToClient conn) { }
    }
}