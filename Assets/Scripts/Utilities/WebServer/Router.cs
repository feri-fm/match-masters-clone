using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.Search;
using UnityEngine;

namespace WebServer
{
    public class Router : ContextHandler
    {
        public string localPattern;
        public List<ContextHandler> handlers = new();

        public bool exactMatch;
        public Pattern pattern;
        public Router parent;

        public Router()
        {
            localPattern = "";
        }
        public Router(string localPattern)
        {
            this.localPattern = localPattern;
        }

        public Router(string localPattern, Router parent)
        {
            this.localPattern = localPattern;
            this.parent = parent;
        }

        public override bool CanHandleContext(Context context)
        {
            return CheckPattern(context.request.httpRequest.RawUrl);
        }

        public override async Task<bool> HandleContext(Context context)
        {
            foreach (var handler in handlers)
            {
                if (handler.CanHandleContext(context))
                {
                    try
                    {
                        context.router = this;
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

        public void Build()
        {
            var hasChild = false;
            foreach (var handler in handlers)
            {
                if (handler is Router router)
                {
                    hasChild = true;
                    router.Build();
                }
            }

            exactMatch = !hasChild;
            var patternString = localPattern;
            var currentParent = parent;
            while (currentParent != null)
            {
                patternString = currentParent.localPattern + patternString;
                currentParent = currentParent.parent;
            }
            pattern = new Pattern(patternString);
        }

        public bool CheckPattern(string url)
        {
            return pattern.Check(url, exactMatch);
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
            return localPattern;
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

        public Router Put(params AsyncMiddlewareCall[] actions) => Put("", actions);
        public Router Put(string path, params AsyncMiddlewareCall[] actions)
            => Use(path, new MiddlewareHandler("PUT", actions.Select(e => new AsyncMiddleware(e)).ToArray()));

        public Router Delete(params AsyncMiddlewareCall[] actions) => Delete("", actions);
        public Router Delete(string path, params AsyncMiddlewareCall[] actions)
            => Use(path, new MiddlewareHandler("DELETE", actions.Select(e => new AsyncMiddleware(e)).ToArray()));
    }

    public class Pattern
    {
        public string patternString;
        public string[] segments;
        public string[] paramKeys;
        public bool[] isParam;

        public Dictionary<string, string> parameters = new();

        public Pattern(string patternString)
        {
            patternString = patternString.Trim();
            if (patternString.Length > 0)
            {
                if (patternString[0] == '/')
                    patternString = patternString.Substring(1);
                if (patternString[patternString.Length - 1] == '/')
                    patternString = patternString.Substring(0, patternString.Length - 1);
                this.patternString = patternString.Trim();
                segments = patternString.Split("/");
                paramKeys = new string[segments.Length];
                isParam = new bool[segments.Length];
                for (int i = 0; i < segments.Length; i++)
                {
                    if (segments[i].StartsWith(":"))
                    {
                        paramKeys[i] = segments[i].Substring(1);
                        isParam[i] = true;
                    }
                    else
                    {
                        paramKeys[i] = "";
                        isParam[i] = false;
                    }
                }
            }
            else
            {
                this.patternString = "";
                segments = new string[] { };
                paramKeys = new string[] { };
                parameters = new();
                isParam = new bool[] { };
            }
        }

        public bool Check(string url, bool exact)
        {
            parameters.Clear();
            url = url.Trim();
            if (url.Length > 0 && url[0] == '/')
                url = url.Substring(1);
            if (url.Length > 0 && url[url.Length - 1] == '/')
                url = url.Substring(0, url.Length - 1);
            var urlSegments = url.Split("/");
            if (exact && urlSegments.Length != segments.Length) return false;
            if (!exact && urlSegments.Length < segments.Length) return false;
            for (int i = 0; i < segments.Length; i++)
            {
                if (isParam[i])
                {
                    parameters[paramKeys[i]] = urlSegments[i];
                }
                else
                {
                    if (segments[i] != urlSegments[i]) return false;
                }
            }
            return true;
        }
    }
}