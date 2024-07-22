using MMC.Server.Models;

namespace MMC.Server
{
    [Controller]
    public class AuthController : Controller
    {
        public override string routeName => "/auth";

        [RoutePost("/register")]
        public void Register() => BuildRoute(
            async (req, res) =>
            {
                var user = await users.CreateNewUser();
                var token = auth.EncodeToken(user.BuildToken);
                await res.AddHeader("x-auth-token", token).Send(user);
            }
        );

        [RoutePost("/login")]
        public void Login() => BuildRoute(
            async (req, res) =>
            {
                var json = await req.ReadJson();
                var username = json["username"].ToString();
                var user = await users.FindUser(username);
                if (user == null)
                {
                    await res.SendError(404, "No user was found for given username");
                    return;
                }
                var token = auth.EncodeToken(user.BuildToken);
                await res.AddHeader("x-auth-token", token).Send(user);
            }
        );

        [RouteGet("/validate")]
        public void Validate() => BuildRoute(
            AuthUser(),
            async (req, res) =>
            {
                var user = req.Get<UserModel>();
                await res.Send(user);
            }
        );
    }
}