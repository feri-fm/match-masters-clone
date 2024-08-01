using UnityEngine;

namespace MMC.Game
{
    public abstract class PipelineBotMiddleware : MonoBehaviour
    {
        public float chance = 1f;

        public Gameplay gameplay { get; private set; }
        public PipelineBot bot { get; private set; }
        public Match3.Game game => gameplay.gameEntity.game;

        public BotAction GetAction(PipelineBot bot, Gameplay gameplay)
        {
            this.bot = bot;
            this.gameplay = gameplay;
            return GetAction();
        }

        protected abstract BotAction GetAction();
    }
}