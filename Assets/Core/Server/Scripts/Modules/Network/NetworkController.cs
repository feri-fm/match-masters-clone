using kcp2k;
using MMC.Network;
using MMC.Server.Models;

namespace MMC.Server
{
    [Controller]
    public class NetworkController : Controller
    {
        public override string routeName => "/network";

        [RouteGet("/request-connection")]
        public void RequestConnection() => BuildRoute(
            async (req, res) =>
            {
                await res.Send(new NetworkConnectionInfo
                {
                    port = NetNetworkManager.instance.GetComponent<KcpTransport>().port,
                    address = "localhost"
                });
            }
        );
    }
}