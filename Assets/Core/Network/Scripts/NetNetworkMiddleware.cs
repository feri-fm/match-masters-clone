using Mirror;
using MMC.Server;
using UnityEngine;

namespace MMC.Network
{
    public class NetNetworkMiddleware : MonoBehaviour
    {
        public NetNetworkManager networkManager { get; private set; }

        public ServerManager serverManager => networkManager.serverManager;

        public void _Setup(NetNetworkManager networkManager)
        {
            this.networkManager = networkManager;
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