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