using System.Threading.Tasks;
using MMC.Match3;

namespace MMC.Game
{
    public class BucketBooster : Booster
    {
        public BucketCommand command;

        protected override async Task Use(GameplayIns ins)
        {
            await base.Use(ins);
            await ins.game.RunCommand(command);
        }
    }
}