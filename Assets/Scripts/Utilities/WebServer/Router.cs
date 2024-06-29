using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServer
{
    public class Router : ContextHandler
    {
        public string path;
        public List<ContextHandler> handlers = new();

        public Router parent;

        public Router()
        {
            path = "";
        }
        public Router(string path)
        {
            this.path = path;
        }

        public Router(string path, Router parent)
        {
            this.path = path;
            this.parent = parent;
        }

        public override bool CanHandleContext(Context context)
        {
            //TODO: use path to match
            var fullPath = GetFullPath();
            return CheckPattern(context.request.httpRequest.RawUrl, fullPath);
        }

        public override async Task<bool> HandleContext(Context context)
        {
            foreach (var handler in handlers)
            {
                if (handler.CanHandleContext(context))
                {
                    try
                    {
                        if (await handler.HandleContext(context))
                        {
                            return true;
                        }
                    }
                    catch (Exception exception)
                    {
                        context.exception = exception;
                    }
                }
            }
            return false;
        }

        public string GetFullPath()
        {
            if (parent != null)
            {
                return parent.GetFullPath() + path;
            }
            return path;
        }

        public bool CheckPattern(string rawUrl, string pattern)
        {
            return rawUrl.StartsWith(pattern);
        }

        public Router In(string path)
        {
            var router = new Router(path, this);
            handlers.Add(router);
            return router;
        }
        public Router Out()
        {
            return parent;
        }

        public Router Use(params ContextHandler[] handlers)
        {
            foreach (var handler in handlers)
                this.handlers.Add(handler);
            return this;
        }
        public Router UsePath(string path, params ContextHandler[] handlers)
        {
            var router = new Router(path, this);
            router.Use(handlers);
            this.handlers.Add(router);
            return this;
        }

        public Router Use(string path, params ContextHandler[] handlers)
        {
            if (path == "" || path == null)
                return Use(handlers);
            return UsePath(path, handlers);
        }
        public override string ToString()
        {
            return path;
        }

        public Router Use(params AsyncMiddlewareCall[] actions) => Use("", actions);
        public Router Use(string path, params AsyncMiddlewareCall[] actions)
            => Use(path, new MiddlewareHandler(actions.Select(e => new AsyncMiddleware(e)).ToArray()));

        public Router Get(params AsyncMiddlewareCall[] actions) => Get("", actions);
        public Router Get(string path, params AsyncMiddlewareCall[] actions)
            => Use(path, new MiddlewareHandler("GET", actions.Select(e => new AsyncMiddleware(e)).ToArray()));

        public Router Post(params AsyncMiddlewareCall[] actions) => Post("", actions);
        public Router Post(string path, params AsyncMiddlewareCall[] actions)
            => Use(path, new MiddlewareHandler("POST", actions.Select(e => new AsyncMiddleware(e)).ToArray()));
    }
}