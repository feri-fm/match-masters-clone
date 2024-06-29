using Mirror;

namespace MMC.Network
{
    public partial class GameNetworkManager
    {
        public override void OnStartServer() { }
        public override void OnStopServer() { }
        public override void OnServerConnect(NetworkConnectionToClient conn) { }
        public override void OnServerDisconnect(NetworkConnectionToClient conn) { }
    }
}