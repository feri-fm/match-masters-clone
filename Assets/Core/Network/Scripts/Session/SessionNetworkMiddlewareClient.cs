using Mirror;
using MMC.Game;
using UnityEngine;

namespace MMC.Network.SessionMiddleware
{
    public partial class SessionNetworkMiddleware
    {
        public override void OnStartClient()
        {
            base.OnStartClient();
            NetworkClient.RegisterHandler<SessionCreatedClientMessage>(OnSessionCreatedClientMessage);
        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            NetworkClient.UnregisterHandler<SessionCreatedClientMessage>();
        }
        public override void OnClientConnect()
        {
            base.OnClientConnect();
            NetworkClient.Send(new AuthServerMessage(gameManager.webRequestManager.token));
        }
        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
        }

        public void OnSessionCreatedClientMessage(SessionCreatedClientMessage msg)
        {
            gameManager.OnSessionCreated();
        }
    }

    public struct SessionCreatedClientMessage : NetworkMessage
    {

    }
}