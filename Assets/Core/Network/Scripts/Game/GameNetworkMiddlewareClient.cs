using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace MMC.Network.GameMiddleware
{
    public class GameNetworkMiddlewareClient : NetNetworkMiddlewareClient<GameNetworkMiddleware>
    {
        public NetGame game;
        public NetPlayer player;
        public NetClient client;
        public List<NetPlayer> players = new List<NetPlayer>();
        public List<NetClient> clients = new List<NetClient>();

        private bool dirtyFetch = false;
        private bool gameStarted = false;

        public RoomData roomData;

        public override void OnStart()
        {
            base.OnStart();
            NetworkClient.RegisterHandler<JoinedRoomClientMessage>(OnJoinedRoomClientMessage);
            NetworkClient.RegisterHandler<UpdateRoomClientMessage>(OnUpdateRoomClientMessage);
            NetworkClient.RegisterHandler<LeaveRoomClientMessage>(OnLeaveRoomClientMessage);
            NetworkClient.RegisterHandler<LeaveGameClientMessage>(OnLeaveGameClientMessage);
        }
        public override void OnStop()
        {
            base.OnStop();
            NetworkClient.UnregisterHandler<JoinedRoomClientMessage>();
            NetworkClient.UnregisterHandler<UpdateRoomClientMessage>();
            NetworkClient.UnregisterHandler<LeaveRoomClientMessage>();
            NetworkClient.UnregisterHandler<LeaveGameClientMessage>();
            OnClearConnection();
        }
        public override void OnConnect()
        {
            base.OnConnect();
        }
        public override void OnDisconnect()
        {
            base.OnDisconnect();
            OnClearConnection();
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
            if (dirtyFetch)
            {
                dirtyFetch = false;
                FetchConnections();
            }
        }

        public void OnGameStart()
        {
            gameManager.gamePanel.OpenPanel();
            client.CmdRequestGameplayStartData();
            game.StartGameClient();
        }
        public void OnGameStop() { }

        public void OnClearConnection()
        {
            foreach (var player in players.ToArray())
            {
                player.OnStopClient();
                Object.Destroy(player.gameObject);
            }
            foreach (var client in clients.ToArray())
            {
                client.OnStopClient();
                Object.Destroy(client.gameObject);
            }
            var g = game;
            if (g != null)
            {
                g.OnStopClient();
                Object.Destroy(g.gameObject);
            }
        }

        public void _SetGame(NetGame game)
        {
            this.game = game;
            MarkDirtyFetch();
        }
        public void _RemoveGame(NetGame game)
        {
            if (this.game == game)
                this.game = null;
            MarkDirtyFetch();
        }
        public void _AddPlayer(NetPlayer player)
        {
            if (!players.Contains(player))
            {
                players.Add(player);
            }
            MarkDirtyFetch();
        }
        public void _RemovePlayer(NetPlayer player)
        {
            players.Remove(player);
            MarkDirtyFetch();
        }
        public void _AddClient(NetClient player)
        {
            if (!clients.Contains(player))
            {
                clients.Add(player);
            }
            MarkDirtyFetch();
        }
        public void _RemoveClient(NetClient player)
        {
            clients.Remove(player);
            MarkDirtyFetch();
        }

        public void MarkDirtyFetch()
        {
            dirtyFetch = true;
        }

        public void FetchConnections()
        {
            if (game != null)
            {
                game.players.Clear();
                foreach (var id in game.playersId)
                {
                    var player = players.Find(e => e.id == id);
                    if (player != null)
                    {
                        game.players.Add(player);
                    }
                }
                client = clients.Find(e => e.isLocalPlayer);
                if (client != null)
                {
                    player = players.Find(e => e.id == client.id);
                }
                foreach (var player in players)
                {
                    player.game = game;
                    player.client = clients.Find(e => e.id == player.id);
                    if (player.client != null)
                        player.client.player = player;
                }
            }
            if (game != null
                && player != null
                && client != null
                && players.Count == game.playersId.Count)
            {
                if (!gameStarted)
                {
                    gameStarted = true;
                    OnGameStart();
                }
            }
            else if (gameStarted)
            {
                gameStarted = false;
                OnGameStop();
            }
        }

        public void JoinGame(NetConfig config)
        {
            NetworkClient.Send(new JoinRoomServerMessage(config.key));
        }
        public void LeaveRoom()
        {
            NetworkClient.Send(new LeaveRoomServerMessage());
        }
        public void LeaveGame()
        {
            NetworkClient.Send(new LeaveGameServerMessage());
        }

        public void OnJoinedRoomClientMessage(JoinedRoomClientMessage msg)
        {

        }
        public void OnUpdateRoomClientMessage(UpdateRoomClientMessage msg)
        {
            roomData = msg.room;
            gameManager.ChangeState();
        }
        public void OnLeaveRoomClientMessage(LeaveRoomClientMessage msg)
        {
            OnClearConnection();
            gameManager.StartMenu();
        }
        public void OnLeaveGameClientMessage(LeaveGameClientMessage msg)
        {
            OnClearConnection();
            gameManager.StartMenu();
        }
    }

    public struct JoinedRoomClientMessage : NetworkMessage
    {

    }
    public struct UpdateRoomClientMessage : NetworkMessage
    {
        public RoomData room;

        public UpdateRoomClientMessage(RoomData room)
        {
            this.room = room;
        }
    }
    public struct LeaveRoomClientMessage : NetworkMessage { }
    public struct LeaveGameClientMessage : NetworkMessage { }
}