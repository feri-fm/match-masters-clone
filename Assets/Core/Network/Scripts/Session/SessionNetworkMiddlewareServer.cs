using System;
using System.Collections.Generic;
using Mirror;
using MMC.Network.GameMiddleware;
using MMC.Server;
using MMC.Server.Models;
using MongoDB.Bson;
using UnityEngine;

namespace MMC.Network.SessionMiddleware
{
    public partial class SessionNetworkMiddleware
    {
        public Dictionary<NetworkConnectionToClient, Session> sessionByConn = new();
        public Dictionary<ObjectId, Session> sessionById = new();

        public List<NetworkConnectionToClient> waitingConnections = new();
        public List<NetworkConnectionToClient> authConnections = new();

        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.RegisterHandler<AuthServerMessage>(OnAuthMessage);
        }
        public override void OnStopServer()
        {
            base.OnStopServer();
            NetworkServer.UnregisterHandler<AuthServerMessage>();
        }
        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
            waitingConnections.Add(conn);
            WaitForAuth(conn);
        }
        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            ClearConnection(conn);
            base.OnServerDisconnect(conn);
        }

        public void ClearConnection(NetworkConnectionToClient conn)
        {
            waitingConnections.Remove(conn);
            authConnections.Remove(conn);
            if (sessionByConn.TryGetValue(conn, out var session))
            {
                sessionByConn.Remove(conn);
                sessionById.Remove(session.user.id);
                OnSessionClosed(session);
            }
        }

        public async void WaitForAuth(NetworkConnectionToClient conn)
        {
            await new WaitForSeconds(2);
            if (waitingConnections.Contains(conn))
            {
                manager.Kick(conn, "No auth token received");
            }
        }

        public void OnSessionCreated(Session session)
        {

        }
        public void OnSessionClosed(Session session)
        {

        }

        public void WithSession(NetworkConnectionToClient conn, Action<Session> action)
        {
            if (sessionByConn.TryGetValue(conn, out var session))
            {
                action.Invoke(session);
            }
        }

        private async void OnAuthMessage(NetworkConnectionToClient conn, AuthServerMessage msg)
        {
            var auth = serverManager.app.Find<AuthService>();
            var users = serverManager.app.Find<UsersService>();
            if (!waitingConnections.Contains(conn))
                return;
            waitingConnections.Remove(conn);
            if (auth.VerifyToken(msg.token, out var json))
            {
                authConnections.Add(conn);
                var data = json.FromJson<UserModel>();
                var user = await users.GetUser(data.id);
                if (user != null)
                {
                    if (authConnections.Contains(conn))
                    {
                        authConnections.Remove(conn);
                        if (sessionById.TryGetValue(user.id, out var otherSession))
                        {
                            manager.Kick(otherSession.conn, "you logged in with another device");
                            ClearConnection(otherSession.conn);
                        }
                        var session = new Session(conn, user);
                        sessionByConn.Add(conn, session);
                        sessionById.Add(user.id, session);
                        conn.Send(new SessionCreatedClientMessage());
                        OnSessionCreated(session);
                    }
                }
                else
                {
                    manager.Kick(conn, "User Not Found");
                }
            }
            else
            {
                manager.Kick(conn, "Invalid token");
            }
        }
    }

    public struct AuthServerMessage : NetworkMessage
    {
        public string token;

        public AuthServerMessage(string token)
        {
            this.token = token;
        }
    }
}