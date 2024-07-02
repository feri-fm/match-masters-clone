using System;
using System.Linq;
using System.Reflection;
using WebServer;

namespace MMC.Server
{
    // map routes to modules, and business logic
    public abstract class Controller : Module
    {
        public abstract string routeName { get; }

        private RouteAttribute lastAttr;

        public Router router;

        protected void BuildRoute(params AsyncMiddlewareCall[] calls)
        {
            router.Use(lastAttr.pattern, new MiddlewareHandler(lastAttr.method, calls.Select(e => new AsyncMiddleware(e)).ToArray()));
        }

        public override void Build()
        {
            base.Build();
            router = app.router.In(routeName);
            BuildRoutes();
        }
        private void BuildRoutes()
        {
            foreach (var method in GetType().GetMethods())
            {
                var attr = method.GetCustomAttribute(typeof(RouteAttribute), true) as RouteAttribute;
                if (attr != null)
                {
                    lastAttr = attr;
                    method.Invoke(this, new object[] { });
                }
            }
            router.Use(async (req, res) =>
            {
                await res.Send(new
                {
                    msg = "not found in " + routeName
                });
            });
        }

        public AsyncMiddlewareCall QueryAll<TModel>()
        {
            return async (req, res) =>
            {

            };
        }

        public AsyncMiddlewareCall QueryOne<TModel>()
        {
            return async (req, res) =>
            {

            };
        }

        public AsyncMiddlewareCall Auth()
        {
            return async (req, res) =>
            {

            };
        }

        public AsyncMiddlewareCall ValidateBody()
        {
            return async (req, res) =>
            {

            };
        }
    }

    public sealed class ControllerAttribute : ModuleAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public abstract class RouteAttribute : Attribute
    {
        public string method = "";
        public string pattern = "";

        public RouteAttribute(string method, string pattern)
        {
            this.method = method;
            this.pattern = pattern;
        }
    }

    public class RouteGetAttribute : RouteAttribute
    {
        public RouteGetAttribute(string pattern) : base("GET", pattern) { }
    }

    public class RoutePostAttribute : RouteAttribute
    {
        public RoutePostAttribute(string pattern) : base("POST", pattern) { }
    }

    public class RoutePutAttribute : RouteAttribute
    {
        public RoutePutAttribute(string pattern) : base("PUT", pattern) { }
    }

    public class RouteDeleteAttribute : RouteAttribute
    {
        public RouteDeleteAttribute(string pattern) : base("DELETE", pattern) { }
    }
}