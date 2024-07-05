namespace MMC.Network.Game
{
    public partial class GameNetworkMiddleware
    {
        public override void OnStartClient() { base.OnStartClient(); }
        public override void OnStartServer() { base.OnStartServer(); }
        public override void OnClientConnect() { base.OnClientConnect(); }
        public override void OnClientDisconnect() { base.OnClientDisconnect(); }
    }
}