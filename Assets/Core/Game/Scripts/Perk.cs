using System.Threading.Tasks;
using UnityEngine;

namespace MMC.Game
{
    public class Perk : MonoBehaviour
    {
        public string key => name;
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

        protected virtual Task<bool> Read(GameplayIns ins) { return Task.FromResult(true); }
        protected virtual Task Use(GameplayIns ins) { return Task.CompletedTask; }
    }

    public class GameplayIns
    {
        public Gameplay gameplay;
        public GameplayReader reader;

        public Match3.Game game => gameplay.gameEntity.game;
        public EngineCore.Engine engine => game.engine;
    }
}