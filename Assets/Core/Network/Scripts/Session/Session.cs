using System;
using Mirror;
using MMC.Server.Models;

namespace MMC.Network.SessionMiddleware
{
    public class Session
    {
        public Guid id;
        public NetworkConnectionToClient conn;
        public UserModel user;

        public Session(NetworkConnectionToClient conn, UserModel user)
        {
            id = Guid.NewGuid();
            this.conn = conn;
            this.user = user;
        }
    }
}