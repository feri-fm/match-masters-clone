using UnityEngine;

namespace MMC.Game.GameplayPlayerViews
{
    public class PerksView : GameplayPlayerViewPart
    {
        public ListLoaderMember list;

        public override void Render()
        {
            base.Render();
            var items = new PerksView[player.perks.Length];
            for (int i = 0; i < items.Length; i++)
                items[i] = this;
            list.UpdateItems(items);
        }
    }
}