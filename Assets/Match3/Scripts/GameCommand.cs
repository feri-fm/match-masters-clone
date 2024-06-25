using System.Threading.Tasks;
using MMC.Core;

namespace MMC.Match3
{
    public abstract class GameCommand
    {
        protected Game game { get; private set; }
        protected Engine engine => game.engine;

        protected abstract Task Run();

        public async Task Run(Game game)
        {
            this.game = game;
            await Run();
        }
    }
}