using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;

namespace NetworkGame
{
    public partial class NetNetworkManager
    {
        public List<NetConfig> configs = new List<NetConfig>();

        public Dictionary<Guid, NetRoom> rooms { get; } = new Dictionary<Guid, NetRoom>();
        public Dictionary<Guid, NetGame> games { get; } = new Dictionary<Guid, NetGame>();

        public Dictionary<NetworkConnectionToClient, NetRoom> roomByConn = new Dictionary<NetworkConnectionToClient, NetRoom>();
        public Dictionary<NetworkConnectionToClient, NetGame> gameByConn = new Dictionary<NetworkConnectionToClient, NetGame>();
        public Dictionary<NetworkConnectionToClient, NetClient> clientByConn = new Dictionary<NetworkConnectionToClient, NetClient>();

        public List<NetworkConnectionToClient> waitingConnections { get; } = new List<NetworkConnectionToClient>();

        public override void OnStartServer()
        {
            base.OnStartServer();
            waitingConnections.Clear();
            RegisterServerHandlers();
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            UnregisterServerHandlers();
        }

        public virtual void RegisterServerHandlers()
        {
            NetworkServer.RegisterHandler<CreateRoomServerMessage>(OnCreateRoomMessage);
            NetworkServer.RegisterHandler<JoinRoomServerMessage>(OnJoinRoomMessage);
            NetworkServer.RegisterHandler<UpdateRoomServerMessage>(OnUpdateRoomMessage);
            NetworkServer.RegisterHandler<LeaveRoomServerMessage>(OnLeaveRoomMessage);
            NetworkServer.RegisterHandler<LaunchRoomServerMessage>(OnLaunchRoomMessage);
            NetworkServer.RegisterHandler<LeaveGameServerMessage>(OnLeaveGameMessage);
        }
        public virtual void UnregisterServerHandlers()
        {
            NetworkServer.UnregisterHandler<CreateRoomServerMessage>();
            NetworkServer.UnregisterHandler<JoinRoomServerMessage>();
            NetworkServer.UnregisterHandler<UpdateRoomServerMessage>();
            NetworkServer.UnregisterHandler<LeaveRoomServerMessage>();
            NetworkServer.UnregisterHandler<LaunchRoomServerMessage>();
            NetworkServer.UnregisterHandler<LeaveGameServerMessage>();
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
            waitingConnections.Add(conn);
            UpdateLobby(conn);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            ClearConnection(conn);
            waitingConnections.Remove(conn);
            base.OnServerDisconnect(conn);
        }

        public override void Update()
        {
            base.Update();
            foreach (var item in rooms)
            {
                item.Value._Update();
            }
        }

        public void ClearConnection(NetworkConnectionToClient conn)
        {
            if (roomByConn.TryGetValue(conn, out var room))
            {
                roomByConn.Remove(conn);
                room.Leave(conn);
            }

            if (clientByConn.TryGetValue(conn, out var client)
                && gameByConn.TryGetValue(conn, out var game))
            {
                clientByConn.Remove(conn);
                // client.player.netIdentity.RemoveClientAuthority();
                client.player.Disconnect();
                gameByConn.Remove(conn);
                if (game.valid)
                    game.OnLeave(client);
            }

            if (!waitingConnections.Contains(conn))
            {
                waitingConnections.Add(conn);
            }

            conn.Send(new ClearConnectionClientMessage());

            UpdateLobby();
        }

        public bool CanRequest(NetworkConnectionToClient conn)
        {
            return waitingConnections.Contains(conn);
        }

        public void SendText(NetworkConnectionToClient conn, string text)
        {
            conn.Send(new TextClientMessage(text));
        }

        public void RemoveRoom(NetRoom room)
        {
            foreach (var player in room.playersByConn)
            {
                roomByConn.Remove(player.Value.conn);
            }
            rooms.Remove(room.id);
            UpdateLobby();
        }

        public void LaunchGame(NetGame game)
        {
            games[game.id] = game;
            for (int i = 0; i < game.players.Count; i++)
            {
                var player = game.players[i];
                if (player.hasClient)
                {
                    gameByConn.Add(player.client.connectionToClient, game);
                    clientByConn.Add(player.client.connectionToClient, player.client);
                }
            }
            for (int i = 0; i < game.players.Count; i++)
            {
                NetworkServer.Spawn(game.players[i].gameObject);
            }
            for (int i = 0; i < game.players.Count; i++)
            {
                var player = game.players[i];
                if (player.hasClient)
                {
                    // player.netIdentity.AssignClientAuthority(player.client.conn);
                }
            }
            NetworkServer.Spawn(game.gameObject);
            game._Spawned();
        }

        public void RemoveGame(NetGame game)
        {
            foreach (var player in game.players)
            {
                if (player.hasClient)
                    gameByConn.Remove(player.client.connectionToClient);
            }
            games.Remove(game.id);
        }

        public void UpdateLobby()
        {
            var msg = new UpdateLobbyClientMessage(rooms.Values.Select(e => e._GetLobbyData()).ToArray());
            foreach (var conn in waitingConnections)
            {
                conn.Send(msg);
            }
        }
        public void UpdateLobby(NetworkConnectionToClient conn)
        {
            var msg = new UpdateLobbyClientMessage(rooms.Values.Select(e => e._GetLobbyData()).ToArray());
            conn.Send(msg);
        }

        public void OnCreateRoomMessage(NetworkConnectionToClient conn, CreateRoomServerMessage msg)
        {
            if (CanRequest(conn))
            {
                var config = configs.Find(e => e.key == msg.configKey);
                if (config != null)
                {
                    var room = config.CreateRoom(conn);
                    room._Setup(config, this);
                    rooms[room.id] = room;

                    roomByConn.Add(conn, room);
                    room.Join(conn);

                    waitingConnections.Remove(conn);

                    UpdateLobby();
                }
                else
                {
                    SendText(conn, "Invalid config key");
                }
            }
        }

        public void OnJoinRoomMessage(NetworkConnectionToClient conn, JoinRoomServerMessage msg)
        {
            if (CanRequest(conn))
            {
                if (rooms.TryGetValue(msg.roomId, out var room))
                {
                    if (room.CanJoin(conn))
                    {
                        roomByConn.Add(conn, room);
                        room.Join(conn);

                        waitingConnections.Remove(conn);

                        UpdateLobby();
                    }
                    else
                    {
                        SendText(conn, "Cannot join the room");
                    }
                }
                else
                {
                    SendText(conn, "Room not found");
                }
            }
        }

        public void OnUpdateRoomMessage(NetworkConnectionToClient conn, UpdateRoomServerMessage msg)
        {
            if (roomByConn.TryGetValue(conn, out var room))
            {
                room._UpdateData(conn, msg);
                UpdateLobby();
            }
        }

        public void OnLeaveRoomMessage(NetworkConnectionToClient conn, LeaveRoomServerMessage msg)
        {
            ClearConnection(conn);
        }

        public void OnLaunchRoomMessage(NetworkConnectionToClient conn, LaunchRoomServerMessage msg)
        {
            if (roomByConn.TryGetValue(conn, out var room))
            {
                room.Launch();
            }
        }

        public void OnLeaveGameMessage(NetworkConnectionToClient conn, LeaveGameServerMessage msg)
        {
            ClearConnection(conn);
        }
    }

    public struct CreateRoomServerMessage : NetworkMessage
    {
        public string configKey;

        public CreateRoomServerMessage(string configKey) { this.configKey = configKey; }
    }

    public struct JoinRoomServerMessage : NetworkMessage
    {
        public Guid roomId;

        public JoinRoomServerMessage(Guid roomId) { this.roomId = roomId; }
    }

    public struct UpdateRoomServerMessage : NetworkMessage
    {
        public string action;
        public string data;

        public UpdateRoomServerMessage(string action, object data)
        {
            this.action = action;
            this.data = data.ToJson();
        }

        public T GetData<T>() => data.FromJson<T>();
    }

    public struct LeaveRoomServerMessage : NetworkMessage { }

    public struct LaunchRoomServerMessage : NetworkMessage { }

    public struct LeaveGameServerMessage : NetworkMessage { }
}
