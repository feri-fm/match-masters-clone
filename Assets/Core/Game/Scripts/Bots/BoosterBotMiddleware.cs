using UnityEngine;

namespace MMC.Game
{
    public abstract class BoosterBotMiddleware<TBooster> : PipelineBotMiddleware
        where TBooster : Booster
    {
        public TBooster booster;

        protected override BotAction GetAction()
        {
            if (bot.player.booster == booster && bot.player.CanUserBooster())
            {
                return GetBoosterAction();
            }
            return null;
        }

        protected abstract BotAction GetBoosterAction();

        private void OnValidate()
        {
            var res = $"{Mathf.FloorToInt(chance * 100)}%";
            res += $" Simple {booster?.key ?? "???"}";
            name = res;
        }
    }
}