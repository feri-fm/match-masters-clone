using System.Collections.Generic;

namespace MMC.Network.GameMiddleware
{
    public class GameNetworkMiddleware : NetNetworkMiddleware<GameNetworkMiddlewareServer, GameNetworkMiddlewareClient>
    {
        public List<NetConfig> configs;

        public NetConfig GetConfig(string key)
        {
            return configs.Find(e => e.key == key);
        }

        public override string GetKey() => "game";

        public override NetNetworkMiddlewareClient CreateClient()
            => new GameNetworkMiddlewareClient();

        public override NetNetworkMiddlewareServer CreateServer()
            => new GameNetworkMiddlewareServer();
    }
}