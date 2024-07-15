namespace MMC.Game
{
    public class BoostersPanel : BasePanel
    {
        public ListLoaderMember items;

        public override void OnRender()
        {
            base.OnRender();
            items.UpdateItems(game.config.boosters);
        }

        [Member]
        public void Back()
        {
            ClosePanel();
        }
    }
}