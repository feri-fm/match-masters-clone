using System;
using System.Threading.Tasks;
using MMC.EngineCore;

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

        public GameCommand Instantiate()
        {
            var commandData = new JsonData();
            JsonData.Save(this, commandData);

            var commandCopy = Activator.CreateInstance(GetType());
            JsonData.Load(commandCopy, commandData);

            return commandCopy as GameCommand;
        }
    }
}