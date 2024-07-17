using UnityEngine;

namespace MMC.Game.GameplayPlayerViews
{
    public class MoveListItem : ListItem<MovesView>
    {
        public GameObjectMember activate;

        protected override void Setup()
        {
            base.Setup();
            activate.SetActive(data.player.moves > index);
        }
    }
}