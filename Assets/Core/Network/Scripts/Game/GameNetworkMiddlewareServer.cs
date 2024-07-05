using Mirror;

namespace MMC.Network.Game
{
    public partial class GameNetworkMiddleware
    {
        public override void OnStopClient() { base.OnStopClient(); }
        public override void OnStopServer() { base.OnStopServer(); }
        public override void OnServerConnect(NetworkConnectionToClient conn) { base.OnServerConnect(conn); }
        public override void OnServerDisconnect(NetworkConnectionToClient conn) { base.OnServerDisconnect(conn); }
    }
}