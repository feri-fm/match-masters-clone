using System.Threading.Tasks;
using MMC.EngineCore;
using MMC.Match3;
using UnityEngine;

namespace MMC.Game
{
    public class HammerPerk : Perk
    {
        public GameObject hammerPrefab;
        public float lifeTime = 0.6f;
        public float hitTime = 0.3f;
        public float waitTime = 0.2f;

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
            if (ins.gameplay.view != null)
            {
                var hammer = Instantiate(hammerPrefab);
                hammer.transform.position = ins.gameplay.view.engineView.GetPosition(tile.position);
                Destroy(hammer.gameObject, lifeTime);
            }
            await ins.game.Wait(hitTime);
            await tile.Hit();
            await ins.game.Wait(waitTime);
            await ins.game.Evaluate();
        }
    }
}