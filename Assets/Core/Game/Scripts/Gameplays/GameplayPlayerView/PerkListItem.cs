using UnityEngine;

namespace MMC.Game.GameplayPlayerViews
{
    public class PerkListItem : ListItem<GameplayPlayerViewPart>
    {
        public ImageMember icon;
        public GameObjectMember canUse;

        protected override void Setup()
        {
            base.Setup();
            icon.sprite = data.player.perks[index].icon;
            canUse.SetActive(!data.player.usedPerks[index]);
        }

        [Member]
        public async void Use()
        {
            if (data.player.isMyPlayer)
            {
                await data.player.UsePerk(index, null);
            }
        }
    }
}