using System;
using System.Collections.Generic;
using System.Linq;
using SwipeableBottomNavigation;

namespace MMC.Game
{
    public class ShopNavPanel : NavigationPanel
    {
        public TextMember dailyExpiresIn;
        public ListLoaderMember dailyDeals;
        public ListLoaderMember timedDeals;
        public ListLoaderMember staticDeals;

        public override void OnRender()
        {
            base.OnRender();
            dailyDeals.UpdateItems(game.user.dailyDeals.deals.Select(e => new DealListItemData()
            {
                deal = game.config.GetDeal(e.Key),
                canPurchase = e.Value > 0,
                purchase = () => game.networkManager.shop.client.ApplyDailyDeal(e.Key),
            }).ToArray());

            timedDeals.UpdateItems(game.config.shop.timedDeals.Select(e => new DealListItemData()
            {
                deal = e.deal,
                canPurchase = game.user.CanApplyTimedDeal(e),
                purchase = () => game.networkManager.shop.client.ApplyTimedDeal(e),
            }));

            staticDeals.UpdateItems(game.config.shop.staticDeals.Select(e => new DealListItemData()
            {
                deal = e,
                canPurchase = true,
                purchase = () => game.networkManager.shop.client.ApplyStaticDeal(e),
            }));
        }

        private void Update()
        {
            dailyExpiresIn.text = (game.user.dailyDeals.expires - DateTime.Now).ToString(@"hh\:mm\:ss");
        }

        public override void OnSelected()
        {
            base.OnSelected();
            network.shop.client.EvaluateShop();
        }
    }
}