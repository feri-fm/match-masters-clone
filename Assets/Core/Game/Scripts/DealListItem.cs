
using UnityEngine.Events;
using UnityEngine.UI;

namespace MMC.Game
{
    public class DealListItem : ListItem<DealListItemData>
    {
        public TextMember key;
        public TextMember price;
        public ImageMember icon;
        public Member<Selectable> purchase;

        protected override void Setup()
        {
            base.Setup();
            key.text = data.deal.key;
            icon.sprite = data.deal.rewards[0].item.icon;
            purchase.value.interactable = data.canPurchase;
            price.text = data.deal.isFree ? "Free" : data.deal.price.GetText();
        }

        [Member]
        public void Purchase()
        {
            if (data.canPurchase)
                data.purchase.Invoke();
        }
    }

    public class DealListItemData
    {
        public Deal deal;
        public bool canPurchase;
        public UnityAction purchase;
    }
}