using System.Threading.Tasks;
using MMC.Match3;

namespace MMC.Game
{
    public class DuckBooster : Booster
    {
        public DuckCommand command;

        protected override async Task Use(GameplayIns ins)
        {
            await base.Use(ins);
            await ins.game.RunCommand(command);
        }
    }
}