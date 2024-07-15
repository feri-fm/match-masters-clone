using System;
using System.Collections.Generic;
using Mirror;
using MMC.Game;
using MMC.Server.Models;
using Newtonsoft.Json.Linq;

namespace MMC.Network.MenuMiddleware
{
    public partial class MenuNetworkMiddleware
    {
        public Dictionary<string, ClientListener> clientListeners = new();

        public override void OnStartClient()
        {
            base.OnStartClient();
            NetworkClient.RegisterHandler<UpdateUserClientMessage>(OnUpdateUserClientMessage);
            NetworkClient.RegisterHandler<SocketClientMessage>(OnSocketClientMessage);
        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            NetworkClient.UnregisterHandler<UpdateUserClientMessage>();
            NetworkClient.UnregisterHandler<SocketClientMessage>();
        }
        public override void OnClientConnect()
        {
            base.OnClientConnect();
        }
        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
        }

        public override void SetupClient()
        {
            base.SetupClient();
            On<UserModel>("update-user", user =>
            {
                gameManager.ChangeState(() =>
                {
                    gameManager.user = user;
                });
            });
        }

        public void Emit(string key, object data)
        {
            NetworkClient.Send(new SocketServerMessage(key, data.ToJson()));
        }

        public void On<T>(string key, Action<T> onData)
        {
            if (!clientListeners.TryGetValue(key, out var listener))
            {
                listener = new ClientListener(key);
                clientListeners.Add(key, listener);
            }
            listener.actions.Add(d => onData.Invoke(d.ToObject<T>()));
        }

        private void OnSocketClientMessage(SocketClientMessage msg)
        {
            if (clientListeners.TryGetValue(msg.key, out var listener))
            {
                listener.Invoke(JObject.Parse(msg.data));
            }
        }

        public void OnUpdateUserClientMessage(UpdateUserClientMessage msg)
        {
            gameManager.ChangeState(() =>
            {
                gameManager.user = msg.userJson.FromJson<UserModel>();
            });
        }

        public void UpdateSelectedItems(string[] items)
        {
            NetworkClient.Send(new UpdateSelectedItemsServerMessage(items));
        }
        public void UnlockBooster(Booster booster) //TODO: this is wrong
        {
            NetworkClient.Send(new UnlockBoosterServerMessage(booster.key));
        }
        public void SetItemCount(string key, int count) //TODO: this is wrong
        {
            NetworkClient.Send(new SetItemCountServerMessage(key, count));
        }
    }

    public struct UpdateUserClientMessage : NetworkMessage
    {
        public string userJson;

        public UpdateUserClientMessage(UserModel user)
        {
            userJson = user.ToJson();
        }
    }

    public struct SocketClientMessage : NetworkMessage
    {
        public string key;
        public string data;

        public SocketClientMessage(string key, string data)
        {
            this.key = key;
            this.data = data;
        }
    }

    public class ClientListener
    {
        public string key;
        public List<Action<JObject>> actions = new();

        public ClientListener(string key)
        {
            this.key = key;
        }

        public void Invoke(JObject data)
        {
            foreach (var action in actions)
            {
                action.Invoke(data);
            }
        }
    }
}