using System.Threading.Tasks;
using MMC.Match3;
using UnityEngine;

namespace MMC.Game
{
    public class Booster : MonoBehaviour
    {
        public string key => name;
        public int requiredScore = 6;
        public Sprite icon;

        public async Task<bool> WriteReader(Gameplay gameplay, GameplayReader reader)
        {
            return await Read(new GameplayIns()
            {
                gameplay = gameplay,
                reader = reader,
            });
        }

        public async Task Apply(Gameplay gameplay, GameplayReader reader)
        {
            await Use(new GameplayIns()
            {
                gameplay = gameplay,
                reader = reader
            });
        }

        protected async Task<bool> ReadTile(GameplayIns ins)
        {
            var tile = await ins.gameplay.PromptTile();
            if (tile != null)
            {
                ins.reader.W("id", tile.id);
                return true;
            }
            return false;
        }

        protected Tile UseTile(GameplayIns ins)
        {
            var id = ins.reader.R<Id>("id");
            var tile = ins.engine.GetEntity<Tile>(id);
            return tile;
        }

        protected virtual Task<bool> Read(GameplayIns ins) { return Task.FromResult(true); }
        protected virtual Task Use(GameplayIns ins) { return Task.CompletedTask; }
    }
}