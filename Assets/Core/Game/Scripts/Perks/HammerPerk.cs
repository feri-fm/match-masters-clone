using System.Threading.Tasks;
using MMC.EngineCore;
using MMC.Match3;

namespace MMC.Game
{
    public class HammerPerk : Perk
    {
        protected override async Task<bool> Read(GameplayIns ins)
        {
            await base.Read(ins);
            var tile = await ins.gameplay.PromptTile();
            if (tile != null)
            {
                ins.reader.W("id", tile.id);
                return true;
            }
            return false;
        }
        protected override async Task Use(GameplayIns ins)
        {
            await base.Use(ins);
            var id = ins.reader.R<Id>("id");
            var tile = ins.engine.GetEntity<Tile>(id);
            await tile.Hit();
            await ins.game.Wait(0.1f);
            await ins.game.Evaluate();
        }
    }
}