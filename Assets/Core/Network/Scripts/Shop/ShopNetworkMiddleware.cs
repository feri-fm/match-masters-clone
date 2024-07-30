namespace MMC.Network.ShopMiddleware
{
    public class ShopNetworkMiddleware : NetNetworkMiddleware<ShopNetworkMiddlewareServer, ShopNetworkMiddlewareClient>
    {
        public override string GetKey() => "shop";

        public override NetNetworkMiddlewareClient CreateClient()
            => new ShopNetworkMiddlewareClient();

        public override NetNetworkMiddlewareServer CreateServer()
            => new ShopNetworkMiddlewareServer();
    }
}