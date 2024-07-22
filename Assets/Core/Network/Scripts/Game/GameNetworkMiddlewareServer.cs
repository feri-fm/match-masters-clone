using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEditor.Rendering;
using UnityEngine;

namespace MMC.Network.GameMiddleware
{
    public class GameNetworkMiddlewareServer : NetNetworkMiddlewareServer<GameNetworkMiddleware>
    {
        public List<NetRoom> rooms = new();
        public List<NetGame> games = new();

        public Dictionary<NetworkConnectionToClient, NetRoom> roomByConn = new();
        public Dictionary<NetworkConnectionToClient, NetGame> gameByConn = new();

        private float lastJoined;

        public override void OnStart()
        {
            base.OnStart();
            rooms.Clear();
            NetworkServer.RegisterHandler<JoinRoomServerMessage>(OnJoinRoomServerMessage);
            NetworkServer.RegisterHandler<LeaveRoomServerMessage>(OnLeaveRoomServerMessage);
            NetworkServer.RegisterHandler<LeaveGameServerMessage>(OnLeaveGameServerMessage);
        }
        public override void OnStop()
        {
            base.OnStop();
            NetworkServer.UnregisterHandler<JoinRoomServerMessage>();
            NetworkServer.UnregisterHandler<LeaveRoomServerMessage>();
            NetworkServer.UnregisterHandler<LeaveGameServerMessage>();
        }
        public override void OnConnect(NetworkConnectionToClient conn)
        {
            base.OnConnect(conn);
        }
        public override void OnDisconnect(NetworkConnectionToClient conn)
        {
            ClearConnection(conn);
            base.OnDisconnect(conn);
        }

        public override void Update()
        {
            base.Update();
            if (rooms.Count > 0 && Time.time > lastJoined + manager.gameService.botJoinTime)
            {
                foreach (var room in rooms)
                {
                    var player = new RoomPlayer(room.config.key, new RoomPlayerData()
                    {
                        id = Guid.NewGuid(),
                        username = "bot_" + UnityEngine.Random.Range(1000, 9999),
                        booster = manager.config.boosters.Random().key,
                        perks = new string[] {
                            manager.config.perks[0].key,
                            manager.config.perks[1].key
                        }
                    });
                    if (room.CanJoin(player))
                    {
                        room.Join(player);
                        room.SendUpdate();

                        if (room.CanLaunch())
                        {
                            QueueLaunch(room);
                        }
                        break;
                    }
                }
            }
        }

        public void ClearConnection(NetworkConnectionToClient conn)
        {
            if (roomByConn.TryGetValue(conn, out var room))
            {
                roomByConn.Remove(conn);
                room.Leave(conn);
                if (room.players.Any(e => e.hasSession))
                    room.SendUpdate();
                else
                    rooms.Remove(room);
            }

            if (gameByConn.TryGetValue(conn, out var game))
            {
                gameByConn.Remove(conn);
                game.Leave(conn);
                if (!game.players.Any(e => e.hasClient))
                {
                    games.Remove(game);
                    game.Destroy();
                }
            }
        }

        public async void QueueLaunch(NetRoom room)
        {
            await new WaitForSeconds(1);
            if (room.CanLaunch())
            {
                rooms.Remove(room);
                foreach (var player in room.players)
                {
                    if (player.hasSession)
                    {
                        roomByConn.Remove(player.session.conn);
                    }
                }

                var game = room.Launch();
                games.Add(game);
                foreach (var player in game.players)
                {
                    if (player.hasClient)
                    {
                        gameByConn[player.client.session.conn] = game;
                    }
                }
            }
        }

        public void OnJoinRoomServerMessage(NetworkConnectionToClient conn, JoinRoomServerMessage msg)
        {
            if (roomByConn.ContainsKey(conn)) return;

            WithSession(conn, session =>
            {
                var config = middleware.configs.Find(e => e.key == msg.config);
                if (config == null)
                {
                    manager.Kick(conn, "Invalid config key");
                    return;
                }

                if (session.user.trophies < config.minTrophies)
                {
                    manager.Kick(conn, "Not enough trophies for selected game");
                    return;
                }

                var joined = false;
                NetRoom room = null;
                var player = new RoomPlayer(msg.config, session);
                foreach (var _room in rooms)
                {
                    if (_room.CanJoin(player))
                    {
                        room = _room;
                        room.Join(player);
                        joined = true;
                        break;
                    }
                }
                if (!joined)
                {
                    room = config.CreateRoom();
                    room.Setup(config);
                    room.Join(player);
                    rooms.Add(room);
                    lastJoined = Time.time;
                }

                roomByConn.Add(conn, room);

                conn.Send(new JoinedRoomClientMessage());
                room.SendUpdate();

                if (room.CanLaunch())
                {
                    QueueLaunch(room);
                }
            });
        }

        public void OnLeaveRoomServerMessage(NetworkConnectionToClient conn, LeaveRoomServerMessage msg)
        {
            if (roomByConn.TryGetValue(conn, out var room))
            {
                ClearConnection(conn);
                conn.Send(new LeaveRoomClientMessage());
            }
        }
        public void OnLeaveGameServerMessage(NetworkConnectionToClient conn, LeaveGameServerMessage msg)
        {
            if (gameByConn.TryGetValue(conn, out var game))
            {
                ClearConnection(conn);
                conn.Send(new LeaveGameClientMessage());
            }
        }
    }

    public struct JoinRoomServerMessage : NetworkMessage
    {
        public string config;

        public JoinRoomServerMessage(string config)
        {
            this.config = config;
        }
    }
    public struct LeaveRoomServerMessage : NetworkMessage { }
    public struct LeaveGameServerMessage : NetworkMessage { }
}