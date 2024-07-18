using System.Threading.Tasks;
using MMC.Network.GameMiddleware;
using MMC.Server.Models;

namespace MMC.Server
{
    [Service]
    public class GamesService : Service
    {
        [UseModule] public GamesRepository repository;

        public async Task<GameModel> CreateNewGame(NetGame game) => await repository.CreateNewGame(game);
    }
}