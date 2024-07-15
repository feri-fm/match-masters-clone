using System.Collections.Generic;

namespace MMC.Network.GameMiddleware
{
    public class GameNetworkMiddleware : NetNetworkMiddleware<GameNetworkMiddlewareServer, GameNetworkMiddlewareClient>
    {
        public List<NetConfig> configs;

        public override NetNetworkMiddlewareClient CreateClient()
        {
            return new GameNetworkMiddlewareClient();
        }

        public override NetNetworkMiddlewareServer CreateServer()
        {
            return new GameNetworkMiddlewareServer();
        }
    }
}