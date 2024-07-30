using Mirror;
using MMC.Game;

namespace MMC.Network.SessionMiddleware
{
    public class SessionNetworkMiddleware : NetNetworkMiddleware<SessionNetworkMiddlewareServer, SessionNetworkMiddlewareClient>
    {
        public override string GetKey() => "session";

        public override NetNetworkMiddlewareClient CreateClient()
            => new SessionNetworkMiddlewareClient();

        public override NetNetworkMiddlewareServer CreateServer()
            => new SessionNetworkMiddlewareServer();
    }
}