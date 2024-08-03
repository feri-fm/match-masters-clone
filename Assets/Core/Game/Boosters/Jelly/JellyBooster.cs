using System.Threading.Tasks;

namespace MMC.Game
{
    public class JellyBooster : Booster
    {
        protected override async Task<bool> Read(GameplayIns ins)
        {
            await base.Read(ins);
            return await ReadTile(ins);
        }
        protected override async Task Use(GameplayIns ins)
        {
            await base.Use(ins);
            var tile = UseTile(ins);
        }
    }
}