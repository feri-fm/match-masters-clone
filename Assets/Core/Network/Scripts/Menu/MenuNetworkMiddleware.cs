using System;
using System.Collections.Generic;

namespace MMC.Network.MenuMiddleware
{
    public class MenuNetworkMiddleware : NetNetworkMiddleware<MenuNetworkMiddlewareServer, MenuNetworkMiddlewareClient>
    {
        public override NetNetworkMiddlewareClient CreateClient()
        {
            return new MenuNetworkMiddlewareClient();
        }

        public override NetNetworkMiddlewareServer CreateServer()
        {
            return new MenuNetworkMiddlewareServer();
        }
    }
}