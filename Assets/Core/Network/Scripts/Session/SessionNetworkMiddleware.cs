using Mirror;
using MMC.Game;

namespace MMC.Network.SessionMiddleware
{
    public class SessionNetworkMiddleware : NetNetworkMiddleware<SessionNetworkMiddlewareServer, SessionNetworkMiddlewareClient>
    {
        public override NetNetworkMiddlewareClient CreateClient()
        {
            return new SessionNetworkMiddlewareClient();
        }

        public override NetNetworkMiddlewareServer CreateServer()
        {
            return new SessionNetworkMiddlewareServer();
        }
    }
}