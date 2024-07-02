using System;
using System.Threading.Tasks;
using UnityEngine;

namespace WebServer
{
    public class ExceptionHandler : ContextHandler
    {
        public override bool CanHandleContext(Context context)
        {
            return context.exception != null;
        }

        public override async Task<bool> HandleContext(Context context)
        {
            Debug.LogError("Exception: " + context.exception);
            await context.response.Send(context.exception.ToString());
            return true;
        }
    }

    public class ExceptionHandler<TException> : ExceptionHandler where TException : Exception
    {
        public ContextHandler handler;

        public ExceptionHandler(ContextHandler handler)
        {
            this.handler = handler;
        }
        public ExceptionHandler(AsyncMiddlewareCall call)
        {
            handler = new MiddlewareHandler(new AsyncMiddleware(call));
        }

        public override bool CanHandleContext(Context context)
        {
            return base.CanHandleContext(context) && context.exception is TException;
        }

        public override async Task<bool> HandleContext(Context context)
        {
            Debug.LogError($"Exception({context.exception.GetType().Name}): {context.exception}");
            return await handler.HandleContext(context);
        }
    }
}