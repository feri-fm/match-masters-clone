using UnityEngine;

namespace MMC.Game.GameplayPlayerViews
{
    public class MovesView : GameplayPlayerViewPart
    {
        public ListLoaderMember list;

        public override void Render()
        {
            base.Render();
            var items = new MovesView[player.totalMoves];
            for (int i = 0; i < items.Length; i++)
                items[i] = this;
            list.UpdateItems(items);
        }
    }
}