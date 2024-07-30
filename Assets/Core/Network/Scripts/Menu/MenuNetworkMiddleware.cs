using System;
using System.Collections.Generic;

namespace MMC.Network.MenuMiddleware
{
    public class MenuNetworkMiddleware : NetNetworkMiddleware<MenuNetworkMiddlewareServer, MenuNetworkMiddlewareClient>
    {
        public override string GetKey() => "menu";

        public override NetNetworkMiddlewareClient CreateClient()
            => new MenuNetworkMiddlewareClient();

        public override NetNetworkMiddlewareServer CreateServer()
            => new MenuNetworkMiddlewareServer();
    }
}