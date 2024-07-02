using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebServer
{

    public class MiddlewareHandler : ContextHandler
    {
        public List<Middleware> middlewares = new();
        public string method;

        public MiddlewareHandler(params Middleware[] middlewares)
        {
            method = "any";
            this.middlewares.AddRange(middlewares);
        }
        public MiddlewareHandler(string method, params Middleware[] middlewares)
        {
            this.method = method;
            this.middlewares.AddRange(middlewares);
        }

        public override bool CanHandleContext(Context context)
        {
            if (context.exception != null) return false;

            if (method == "any")
                return true;
            if (context.request.httpRequest.HttpMethod == method)
                return true;
            return false;
        }

        public override async Task<bool> HandleContext(Context context)
        {
            foreach (var middleware in middlewares)
            {
                await middleware.HandleContext(context);
                if (context.closed) return true;
            }
            return true;
        }
    }
}
