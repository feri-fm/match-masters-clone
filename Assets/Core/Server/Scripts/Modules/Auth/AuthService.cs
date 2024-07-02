using WebServer;

namespace MMC.Server
{
    [Service]
    public class AuthService : Service
    {
        public void Register() { }

        public void Auth(string token) { }

        public void Login(string username, string password) { }
    }
}