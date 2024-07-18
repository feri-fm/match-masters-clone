using MMC.Server.Models;

namespace MMC.Server
{
    [Controller]
    public class GamesController : Controller
    {
        public override string routeName => "/games";

        [RouteGet("/")]
        public void GetGames() => BuildRoute(
            AuthUser(),
            QueryAll<GameModel>(),
            async (req, res) =>
            {
                await res.Send(new { games = "games data!" });
            }
        );

        [RouteGet("/:id")]
        public void GetGame() => BuildRoute(
            AuthUser(),
            QueryAll<GameModel>(),
            async (req, res) =>
            {
                await res.Send(new { game = "got game at " + req.context.router.pattern.parameters["id"] });
            }
        );
    }
}