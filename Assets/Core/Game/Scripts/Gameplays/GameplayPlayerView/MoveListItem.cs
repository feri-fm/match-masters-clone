using UnityEngine;

namespace MMC.Game.GameplayPlayerViews
{
    public class MoveListItem : ListItem<MovesView>
    {
        public GameObjectMember activate;

        protected override void Setup()
        {
            base.Setup();
            if (data.player.isTurn)
                activate.SetActive(data.player.moves > index);
            else
            {
                if (data.player.gameplay is TwoPlayerGameplay twoPlayerGameplay)
                {
                    if (twoPlayerGameplay.turn == 0)
                        activate.SetActive(true);
                    else
                        activate.SetActive(false);
                }
                else
                {
                    activate.SetActive(true);
                }
            }
        }
    }
}