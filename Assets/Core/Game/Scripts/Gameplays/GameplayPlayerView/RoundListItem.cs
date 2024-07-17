using UnityEngine;

namespace MMC.Game.GameplayPlayerViews
{
    public class RoundListItem : ListItem<RoundsView>
    {
        public TextMember text;
        public GameObjectMember activate;
        public GameObjectMember current;

        protected override void Setup()
        {
            base.Setup();
            text.text = $"{index + 1}";
            activate.SetActive(data.player.round >= index);
            current.SetActive(data.player.round == index);
        }
    }
}