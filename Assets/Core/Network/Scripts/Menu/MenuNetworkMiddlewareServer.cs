using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using MMC.Network.SessionMiddleware;
using MMC.Server.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json.Linq;

namespace MMC.Network.MenuMiddleware
{
    public partial class MenuNetworkMiddleware
    {
        public Dictionary<string, ServerListener> serverListeners = new();

        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.RegisterHandler<UpdateSelectedItemsServerMessage>(OnUpdateSelectedItemsServerMessage);
            NetworkServer.RegisterHandler<UnlockBoosterServerMessage>(OnUnlockBoosterServerMessage);
            NetworkServer.RegisterHandler<SetItemCountServerMessage>(OnSetItemCountServerMessage);
            NetworkServer.RegisterHandler<SocketServerMessage>(OnSocketServerMessage);
        }
        public override void OnStopServer()
        {
            base.OnStopServer();
            NetworkServer.UnregisterHandler<UpdateSelectedItemsServerMessage>();
            NetworkServer.UnregisterHandler<UnlockBoosterServerMessage>();
            NetworkServer.UnregisterHandler<SetItemCountServerMessage>();
            NetworkServer.UnregisterHandler<SocketServerMessage>();
        }
        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
        }
        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
        }

        public override void SetupServer()
        {
            base.SetupServer();
            On<string[]>("update-selected-items", (session, data) =>
            {

            });
        }

        public void Emit(Session session, string key, object data)
        {
            session.conn.Send(new SocketClientMessage(key, data.ToJson()));
        }

        public void On<T>(string key, Action<Session, T> onData)
        {
            if (!serverListeners.TryGetValue(key, out var listener))
            {
                listener = new ServerListener(key);
                serverListeners.Add(key, listener);
            }
            listener.actions.Add((s, d) => onData.Invoke(s, d.ToObject<T>()));
        }

        private void OnSocketServerMessage(NetworkConnectionToClient conn, SocketServerMessage msg)
        {
            WithSession(conn, session =>
            {
                if (serverListeners.TryGetValue(msg.key, out var listener))
                {
                    listener.Invoke(session, JObject.Parse(msg.data));
                }
            });
        }

        public void OnUpdateSelectedItemsServerMessage(NetworkConnectionToClient conn, UpdateSelectedItemsServerMessage msg)
        {
            WithSession(conn, async session =>
            {
                var user = session.user;
                if (user.inventory.HasItems(msg.items))
                {
                    user.selectedItems = msg.items.ToList();
                    await user.Update(e => e.Set(u => u.selectedItems, user.selectedItems));
                }
                conn.Send(new UpdateUserClientMessage(user));
            });
        }
        public void OnUnlockBoosterServerMessage(NetworkConnectionToClient conn, UnlockBoosterServerMessage msg)
        {
            WithSession(conn, async session =>
            {
                var user = session.user;
                if (!user.inventory.HasItem(msg.key))
                {
                    user.inventory.AddItem(msg.key);
                    user.inventory.SetCount(msg.key, 1);
                    await user.Update(e => e.Set(u => u.inventory, user.inventory));
                }
                conn.Send(new UpdateUserClientMessage(user));
            });
        }
        public void OnSetItemCountServerMessage(NetworkConnectionToClient conn, SetItemCountServerMessage msg)
        {
            WithSession(conn, async session =>
            {
                var user = session.user;
                if (user.inventory.HasItem(msg.key))
                {
                    user.inventory.SetCount(msg.key, msg.count);
                    await user.Update(e => e.Set(u => u.inventory.counts, user.inventory.counts));
                }
                conn.Send(new UpdateUserClientMessage(user));
            });
        }
    }

    public struct UpdateSelectedItemsServerMessage : NetworkMessage
    {
        public string[] items; public UpdateSelectedItemsServerMessage(string[] items) { this.items = items; }
    }
    public struct UnlockBoosterServerMessage : NetworkMessage
    {
        public string key; public UnlockBoosterServerMessage(string key) { this.key = key; }
    }
    public struct SetItemCountServerMessage : NetworkMessage
    {
        public string key; public int count; public SetItemCountServerMessage(string key, int count) { this.key = key; this.count = count; }
    }

    public struct SocketServerMessage : NetworkMessage
    {
        public string key;
        public string data;

        public SocketServerMessage(string key, string data)
        {
            this.key = key;
            this.data = data;
        }
    }


    public class ServerListener
    {
        public string key;
        public List<Action<Session, JObject>> actions = new();

        public ServerListener(string key)
        {
            this.key = key;
        }

        public void Invoke(Session session, JObject data)
        {
            foreach (var action in actions)
            {
                action.Invoke(session, data);
            }
        }
    }
}