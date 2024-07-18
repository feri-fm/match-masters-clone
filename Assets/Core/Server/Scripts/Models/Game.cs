using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JWT.Builder;
using MMC.Game;
using MMC.Network.GameMiddleware;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MMC.Server.Models
{
    public class GameModel : Model<GameModel>
    {
        public GamePlayerModel[] players;
        public int winnerIndex;
        public GameStateModel state;

        public async Task FinishGame(NetGame game)
        {
            state = GameStateModel.Finished;
            players[0].score = game.gameplay.myPlayer.score;
            players[1].score = game.gameplay.opponentPlayer.score;
            winnerIndex = players[0].score > players[1].score ? 0 : players[0].score < players[1].score ? 1 : -1;
            await Update(e => e.Set(m => m.state, state)
                .Set(m => m.players, players));
        }
        public async Task CancelGame(NetGame game)
        {
            state = GameStateModel.Canceled;
            players[0].score = game.gameplay.myPlayer.score;
            players[1].score = game.gameplay.opponentPlayer.score;
            winnerIndex = players[0].score > players[1].score ? 0 : players[0].score < players[1].score ? 1 : -1;
            await Update(e => e.Set(m => m.state, state)
                .Set(m => m.players, players));
        }
    }
    public class GamePlayerModel
    {
        public GameUserModel user;
        public string[] items;
        public int score;
    }
    public class GameUserModel
    {
        [JsonConverter(typeof(ObjectIdConverter))] public ObjectId userId;
    }

    public enum GameStateModel
    {
        Started, Canceled, Finished
    }
}