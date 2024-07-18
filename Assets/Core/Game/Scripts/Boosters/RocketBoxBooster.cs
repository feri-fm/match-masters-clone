using System.Threading.Tasks;

namespace MMC.Game
{
    public class RocketBoxBooster : Booster
    {
        public RocketBoxCommand command;

        protected override async Task Use(GameplayIns ins)
        {
            await base.Use(ins);
            await ins.game.RunCommand(command);
        }
    }
}