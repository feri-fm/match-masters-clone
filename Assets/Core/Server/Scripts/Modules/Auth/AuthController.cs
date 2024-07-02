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
                await res.Send(new { username = "register_boo" });
            }
        );

        [RoutePost("/login")]
        public void Login() => BuildRoute(
            ValidateBody(),
            QueryOne<UserModel>(),
            async (req, res) =>
            {
                var stream = req.httpRequest.InputStream;
                var length = req.httpRequest.ContentLength64;
                var bytes = new byte[length];
                await stream.ReadAsync(bytes, 0, (int)length);
                var text = req.httpRequest.ContentEncoding.GetString(bytes);
                await res.Send(new { username = "boo", data = text });
            }
        );
    }
}