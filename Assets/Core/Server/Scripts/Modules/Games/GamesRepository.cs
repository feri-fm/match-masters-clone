using System.Linq;
using System.Threading.Tasks;
using MMC.Network.GameMiddleware;
using MMC.Server.Models;

namespace MMC.Server
{
    [Repository]
    public class GamesRepository : Repository
    {
        public async Task<GameModel> CreateNewGame(NetGame netGame)
        {
            var game = new GameModel
            {
                players = netGame.players.Select(e => new GamePlayerModel()
                {
                    user = e.hasClient ? new GameUserModel()
                    {
                        userId = e.client.session.user.id
                    } : null,
                    //TODO: not good using index
                    items = new string[] { e.booster.key, e.perks[0].key, e.perks[1].key },
                    score = 0,
                }).ToArray(),
                winnerIndex = -1,
                state = GameStateModel.Started,
            };
            await database.games.InsertOneAsync(game);
            game._Setup(database);
            return game;
        }
    }
}