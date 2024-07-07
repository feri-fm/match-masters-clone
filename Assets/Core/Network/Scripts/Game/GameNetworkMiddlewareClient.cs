using System.Collections.Generic;
using Mirror;

namespace MMC.Network.GameMiddleware
{
    public partial class GameNetworkMiddleware
    {
        public NetGame game;
        public NetPlayer player;
        public NetClient client;
        public List<NetPlayer> players = new List<NetPlayer>();
        public List<NetClient> clients = new List<NetClient>();

        private bool dirtyFetch = false;
        private bool gameStarted = false;

        public RoomPlayerData[] roomPlayers;

        public override void OnStartClient()
        {
            base.OnStartClient();
            NetworkClient.RegisterHandler<JoinedRoomClientMessage>(OnJoinedRoomClientMessage);
            NetworkClient.RegisterHandler<UpdateRoomClientMessage>(OnUpdateRoomClientMessage);
            NetworkClient.RegisterHandler<StartGameClientMessage>(OnStartGameClientMessage);
            NetworkClient.RegisterHandler<LeaveRoomClientMessage>(OnLeaveRoomClientMessage);
            NetworkClient.RegisterHandler<LeaveGameClientMessage>(OnLeaveGameClientMessage);
        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            NetworkClient.UnregisterHandler<JoinedRoomClientMessage>();
            NetworkClient.UnregisterHandler<UpdateRoomClientMessage>();
            NetworkClient.UnregisterHandler<StartGameClientMessage>();
            NetworkClient.UnregisterHandler<LeaveRoomClientMessage>();
            NetworkClient.UnregisterHandler<LeaveGameClientMessage>();
            OnClearConnection();
        }
        public override void OnClientConnect()
        {
            base.OnClientConnect();
        }
        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            OnClearConnection();
        }

        private void LateUpdate()
        {
            if (dirtyFetch)
            {
                dirtyFetch = false;
                FetchConnections();
            }
        }

        public void OnGameStart() { }
        public void OnGameStop() { }

        public void OnClearConnection()
        {
            foreach (var player in players.ToArray())
            {
                player.OnStopClient();
                Destroy(player.gameObject);
            }
            foreach (var client in clients.ToArray())
            {
                client.OnStopClient();
                Destroy(client.gameObject);
            }
            var g = game;
            if (g != null)
            {
                g.OnStopClient();
                Destroy(g.gameObject);
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
            roomPlayers = msg.players;
            gameManager.ChangeState();
        }
        public void OnStartGameClientMessage(StartGameClientMessage msg)
        {
            gameManager.gamePanel.OpenPanel();
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
        public RoomPlayerData[] players;

        public UpdateRoomClientMessage(RoomPlayerData[] players)
        {
            this.players = players;
        }
    }
    public struct StartGameClientMessage : NetworkMessage { }
    public struct LeaveRoomClientMessage : NetworkMessage { }
    public struct LeaveGameClientMessage : NetworkMessage { }
}