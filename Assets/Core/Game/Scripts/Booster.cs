using System.Threading.Tasks;
using UnityEngine;

namespace MMC.Game
{
    public class Booster : MonoBehaviour
    {
        public string key => name;
        public int requiredScore = 6;
        public Sprite icon;

        protected Gameplay gameplay;
        protected Match3.Game game => gameplay.gameEntity.game;

        public async Task Use(Gameplay gameplay)
        {
            this.gameplay = gameplay;
            await Use();
        }

        protected virtual Task Use() { return Task.CompletedTask; }
    }
}