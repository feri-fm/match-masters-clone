using System.Threading.Tasks;
using MMC.Match3;

namespace MMC.Game
{
    public class BucketBooster : Booster
    {
        public BucketCommand command;

        protected override async Task Use()
        {
            await base.Use();
            await game.RunCommand(command);
        }
    }
}