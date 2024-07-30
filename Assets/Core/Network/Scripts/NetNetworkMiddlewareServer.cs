using System;
using Mirror;
using MMC.Network.SessionMiddleware;
using MMC.Server;
using MMC.Server.Models;

namespace MMC.Network
{
    public class NetNetworkMiddlewareServer
    {
        public NetNetworkMiddleware middleware { get; private set; }
        public NetNetworkManager manager => middleware.manager;
        public ServerManager serverManager => manager.serverManager;

        public void _Setup(NetNetworkMiddleware middleware)
        {
            this.middleware = middleware;
            Setup();
        }

        public virtual void Setup() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }
        public virtual void OnStart() { }
        public virtual void OnStop() { }
        public virtual void OnConnect(NetworkConnectionToClient conn) { }
        public virtual void OnDisconnect(NetworkConnectionToClient conn) { }

        public void Emit(Session session, string key, object data)
        {
            manager.ServerEmit(session, middleware.GetKey(key), data);
        }
        public void On<T>(string key, Action<Session, T> onData)
        {
            manager.ServerOn(middleware.GetKey(key), onData);
        }

        public void EmitUpdateUser(Session session) => EmitUpdateUser(session, session.user);
        public void EmitUpdateUser(Session session, UserModel user)
        {
            Emit(session, "update-user", user);
        }

        public void WithSession(NetworkConnectionToClient conn, Action<Session> action)
        {
            manager.session.server._WithSession(conn, action);
        }
    }

    public class NetNetworkMiddlewareServer<TMiddleware> : NetNetworkMiddlewareServer
        where TMiddleware : NetNetworkMiddleware
    {
        public new TMiddleware middleware => base.middleware as TMiddleware;
    }
}