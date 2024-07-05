using Mirror;
using MMC.Game;

namespace MMC.Network.Session
{
    public partial class SessionNetworkMiddleware
    {
        public override void OnStartClient()
        {
            base.OnStartClient();
            NetworkClient.RegisterHandler<KickClientMessage>(OnKickClientMessage);
            NetworkClient.RegisterHandler<SessionCreatedClientMessage>(OnSessionCreatedClientMessage);
        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            NetworkClient.UnregisterHandler<KickClientMessage>();
            NetworkClient.UnregisterHandler<SessionCreatedClientMessage>();
        }
        public override void OnClientConnect()
        {
            base.OnClientConnect();
            NetworkClient.Send(new AuthServerMessage(GameManager.instance.webRequestManager.token));
        }
        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
        }

        public void OnKickClientMessage(KickClientMessage msg)
        {
            Popup.ShowAlert(msg.message);
        }
        public void OnSessionCreatedClientMessage(SessionCreatedClientMessage msg)
        {
            GameManager.instance.SessionCreated();
        }
    }

    public struct KickClientMessage : NetworkMessage
    {
        public string message;

        public KickClientMessage(string message)
        {
            this.message = message;
        }
    }

    public struct SessionCreatedClientMessage : NetworkMessage
    {

    }
}