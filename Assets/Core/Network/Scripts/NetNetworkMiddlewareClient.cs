using System;
using MMC.Game;
using MMC.Server.Models;

namespace MMC.Network
{
    public class NetNetworkMiddlewareClient
    {
        public NetNetworkMiddleware middleware { get; private set; }
        public NetNetworkManager manager => middleware.manager;
        public GameManager gameManager => GameManager.instance;

        public virtual void Setup() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }
        public virtual void OnStart() { }
        public virtual void OnStop() { }
        public virtual void OnConnect() { }
        public virtual void OnDisconnect() { }

        public void _Setup(NetNetworkMiddleware middleware)
        {
            this.middleware = middleware;

            On<UserModel>("update-user", user =>
            {
                gameManager.ChangeState(() =>
                {
                    gameManager.user = user;
                });
            });
            On<string>("message", message =>
            {
                Popup.ShowAlert(message);
            });

            Setup();
        }

        public void Emit(string key, object data)
        {
            manager.ClientEmit(middleware.GetKey(key), data);
        }
        public void On<T>(string key, Action<T> onData)
        {
            manager.ClientOn(middleware.GetKey(key), onData);
        }
    }

    public class NetNetworkMiddlewareClient<TMiddleware> : NetNetworkMiddlewareClient
        where TMiddleware : NetNetworkMiddleware
    {
        public new TMiddleware middleware => base.middleware as TMiddleware;
    }
}