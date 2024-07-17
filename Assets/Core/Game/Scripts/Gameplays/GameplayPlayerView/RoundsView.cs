using UnityEngine;

namespace MMC.Game.GameplayPlayerViews
{
    public class RoundsView : GameplayPlayerViewPart
    {
        public ListLoaderMember list;

        public override void Render()
        {
            base.Render();
            var items = new RoundsView[player.totalRounds];
            for (int i = 0; i < items.Length; i++)
                items[i] = this;
            list.UpdateItems(items);
        }
    }
}