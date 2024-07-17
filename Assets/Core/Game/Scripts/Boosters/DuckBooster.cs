using System.Threading.Tasks;
using MMC.Match3;

namespace MMC.Game
{
    public class DuckBooster : Booster
    {
        public DuckCommand command;

        protected override async Task Use()
        {
            await base.Use();
            await game.RunCommand(command);
        }
    }
}