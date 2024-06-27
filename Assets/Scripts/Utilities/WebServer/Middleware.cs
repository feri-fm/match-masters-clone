using System;
using System.Threading.Tasks;

namespace WebServer
{
    public abstract class Middleware
    {
        public abstract Task HandleContext(Context context);
    }

    public class AsyncMiddleware : Middleware
    {
        public AsyncMiddlewareCall action;

        public AsyncMiddleware(AsyncMiddlewareCall action)
        {
            this.action = action;
        }

        public override async Task HandleContext(Context context)
        {
            await action.Invoke(context.request, context.response);
        }
    }

    public delegate Task AsyncMiddlewareCall(Request request, Response response);
}