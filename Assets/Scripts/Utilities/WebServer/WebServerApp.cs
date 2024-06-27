using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace WebServer
{
    public class WebServerApp
    {
        private ContextHandler handler;
        private HttpListener listener;
        private CancellationTokenSource cancellationTokenSource;

        public void Setup(ContextHandler handler)
        {
            this.handler = handler;
        }

        public void Stop()
        {
            cancellationTokenSource?.Cancel();
            listener?.Stop();
            cancellationTokenSource = null;
            listener = null;
        }

        public void Listen(ushort port) => Listen($"http://*:{port}/");
        public void Listen(params string[] prefixes)
        {
            Stop();

            listener = new HttpListener();
            foreach (var prefix in prefixes)
                listener.Prefixes.Add(prefix);

            listener.Start();

            cancellationTokenSource = new CancellationTokenSource();
            _ = StartListeningForRequests(cancellationTokenSource.Token);
        }

        private async Task StartListeningForRequests(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var httpListenerContext = await listener.GetContextAsync();
                var context = new Context(httpListenerContext);
                _ = HandlerContext(context);
            }
        }

        private async Task HandlerContext(Context context)
        {
            try
            {
                await handler.HandleContext(context);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                throw;
            }
        }
    }
}
