using System.Net.WebSockets;
using System.Threading.Tasks;

namespace WebServer
{

    // unity mono doesn't support this, it doesn't work
    public class SocketHandler : ContextHandler
    {
        public WebSocket webSocket;

        public override bool CanHandleContext(Context context)
        {
            if (!context.request.httpRequest.IsWebSocketRequest) return false;
            return true;
        }

        public override async Task<bool> HandleContext(Context context)
        {
            var webSocketContext = await context.httpListenerContext.AcceptWebSocketAsync(null);
            webSocket = webSocketContext.WebSocket;

            StartSending();
            StartReceiving();

            await context.response.Send(new { hello = "World" });

            return true;
        }

        private void StartSending()
        {

        }
        private void StartReceiving()
        {

        }
    }
}