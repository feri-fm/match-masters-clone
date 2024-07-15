using System;
using Mirror;
using MMC.Server.Models;

namespace MMC.Network.SessionMiddleware
{
    public class Session
    {
        public Guid id { get; }
        public NetworkConnectionToClient conn { get; }
        public UserModel user { get; }

        public Session(NetworkConnectionToClient conn, UserModel user)
        {
            id = Guid.NewGuid();
            this.conn = conn;
            this.user = user;
        }
    }
}