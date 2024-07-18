using System.Threading.Tasks;

namespace MMC.Game
{
    public class MagicHatBooster : Booster
    {
        public HatCommand command;

        protected override async Task Use(GameplayIns ins)
        {
            await base.Use(ins);
            await ins.game.RunCommand(command);
        }
    }
}