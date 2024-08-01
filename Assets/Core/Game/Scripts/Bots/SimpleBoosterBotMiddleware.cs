namespace MMC.Game
{
    public class SimpleBoosterBotMiddleware : BoosterBotMiddleware<Booster>
    {
        protected override BotAction GetBoosterAction()
        {
            return new UseBoosterAction();
        }
    }
}