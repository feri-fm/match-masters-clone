using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace NetworkGame
{
    public partial class NetNetworkManager
    {
        public override void OnStartClient()
        {
            base.OnStartClient();
            RegisterClientHandlers();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            UnregisterClientHandlers();
        }

        public virtual void RegisterClientHandlers()
        {
            NetworkClient.RegisterHandler<TextClientMessage>(OnTextMessage);
            NetworkClient.RegisterHandler<JoinRoomClientMessage>(OnJoinRoomMessage);
            NetworkClient.RegisterHandler<UpdateRoomClientMessage>(OnUpdateRoomMessage);
            NetworkClient.RegisterHandler<UpdateLobbyClientMessage>(OnUpdateLobbyMessage);
            NetworkClient.RegisterHandler<ClearConnectionClientMessage>(OnClearConnectionMessage);
        }
        public virtual void UnregisterClientHandlers()
        {
            NetworkClient.UnregisterHandler<TextClientMessage>();
            NetworkClient.UnregisterHandler<JoinRoomClientMessage>();
            NetworkClient.UnregisterHandler<UpdateRoomClientMessage>();
            NetworkClient.UnregisterHandler<UpdateLobbyClientMessage>();
            NetworkClient.UnregisterHandler<ClearConnectionClientMessage>();
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
        }

        public void SendCreateRoom(string configKey)
        {
            NetworkClient.Send(new CreateRoomServerMessage(configKey));
        }
        public void SendJoinRoom(Guid roomId)
        {
            NetworkClient.Send(new JoinRoomServerMessage(roomId));
        }
        public void SendUpdateRoom(string action, object data)
        {
            NetworkClient.Send(new UpdateRoomServerMessage(action, data));
        }
        public void SendLeaveRoom()
        {
            NetworkClient.Send(new LeaveRoomServerMessage());
        }
        public void SendLaunchRoom()
        {
            NetworkClient.Send(new LaunchRoomServerMessage());
        }
        public void SendLeaveGame()
        {
            NetworkClient.Send(new LeaveGameServerMessage());
        }

        public virtual void OnTextMessage(TextClientMessage msg) { }

        public virtual void OnJoinRoomMessage(JoinRoomClientMessage msg) { }

        public virtual void OnUpdateRoomMessage(UpdateRoomClientMessage msg) { }

        public virtual void OnUpdateLobbyMessage(UpdateLobbyClientMessage msg) { }

        public virtual void OnClearConnectionMessage(ClearConnectionClientMessage msg) { }
    }

    public struct TextClientMessage : NetworkMessage
    {
        public string text;

        public TextClientMessage(string text) { this.text = text; }
    }

    public struct JoinRoomClientMessage : NetworkMessage
    {

    }

    public struct UpdateRoomClientMessage : NetworkMessage
    {
        public string action;
        public string playerId;
        public string data;

        public UpdateRoomClientMessage(string action, string playerId, object data)
        {
            this.action = action;
            this.playerId = playerId;
            this.data = data.ToJson();
        }

        public T GetData<T>() => data.FromJson<T>();
    }

    public struct UpdateLobbyClientMessage : NetworkMessage
    {
        public NetRoomData[] rooms;

        public UpdateLobbyClientMessage(NetRoomData[] rooms)
        {
            this.rooms = rooms;
        }
    }

    public struct ClearConnectionClientMessage : NetworkMessage
    {

    }
}
