using System.Threading.Tasks;
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

        protected virtual Task<bool> Read(GameplayIns ins) { return Task.FromResult(true); }
        protected virtual Task Use(GameplayIns ins) { return Task.CompletedTask; }
    }
}