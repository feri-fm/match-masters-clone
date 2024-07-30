using System;
using System.Linq;
using System.Threading.Tasks;
using MMC.Game;
using MMC.Server.Models;

namespace MMC.Network.ShopMiddleware
{
    public class ShopNetworkMiddlewareServer : NetNetworkMiddlewareServer<ShopNetworkMiddleware>
    {
        public override void Setup()
        {
            base.Setup();
            On<int>("evaluate-shop", async (session, _) =>
            {
                if (await EvaluateDailyDeals(session.user))
                {
                    EmitUpdateUser(session);
                }
            });
            On<string>("apply-daily-deal", async (session, key) =>
            {
                var deal = manager.config.GetDeal(key);
                if (await session.user.TryApplyDailyDeal(deal))
                {
                    EmitUpdateUser(session);
                }
            });
            On<string>("apply-timed-deal", async (session, key) =>
            {
                var deal = manager.config.shop.GetTimedDeal(key);
                if (await session.user.TryApplyTimedDeal(deal))
                {
                    EmitUpdateUser(session);
                }
            });
            On<string>("apply-static-deal", async (session, key) =>
            {
                var deal = manager.config.shop.GetStaticDeal(key);
                if (await session.user.TryApplyDeal(deal))
                {
                    EmitUpdateUser(session);
                }
            });
        }

        public async Task<bool> EvaluateDailyDeals(UserModel user)
        {
            if (DateTime.Now > user.dailyDeals.expires)
            {
                var config = manager.config.shop.GetDailyDeals(user.trophies);
                user.dailyDeals = GenerateDailyDeals(config);
                await user.Update(e => e.Set(m => m.dailyDeals, user.dailyDeals));
                return true;
            }
            return false;
        }

        public DailyDealsModel GenerateDailyDeals(DailyDeals config)
        {
            var res = new DailyDealsModel();
            var deals = config.deals.ToArray().Shuffle();
            for (int i = 0; i < deals.Length && i < config.pick; i++)
            {
                var deal = deals[i];
                res.deals[deal.deal.key] = deal.count;
            }
            res.expires = DateTime.Now + TimeSpan.FromDays(1);
            return res;
        }
    }
}