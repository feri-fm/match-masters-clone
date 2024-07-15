using Mirror;
using MMC.Game;
using UnityEngine;

namespace MMC.Network.SessionMiddleware
{
    public class SessionNetworkMiddlewareClient : NetNetworkMiddlewareClient<SessionNetworkMiddleware>
    {
        public override void OnStart()
        {
            base.OnStart();
            NetworkClient.RegisterHandler<SessionCreatedClientMessage>(OnSessionCreatedClientMessage);
        }
        public override void OnStop()
        {
            base.OnStop();
            NetworkClient.UnregisterHandler<SessionCreatedClientMessage>();
        }
        public override void OnConnect()
        {
            base.OnConnect();
            NetworkClient.Send(new AuthServerMessage(gameManager.webRequestManager.token));
        }
        public override void OnDisconnect()
        {
            base.OnDisconnect();
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