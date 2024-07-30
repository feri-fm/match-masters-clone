using MMC.Game;

namespace MMC.Network.ShopMiddleware
{
    public class ShopNetworkMiddlewareClient : NetNetworkMiddlewareClient<ShopNetworkMiddleware>
    {
        public void EvaluateShop() => Emit("evaluate-shop", 0);
        public void ApplyDailyDeal(string key) => Emit("apply-daily-deal", key);
        public void ApplyTimedDeal(TimedDeal timedDeal) => Emit("apply-timed-deal", timedDeal.key);
        public void ApplyStaticDeal(Deal deal) => Emit("apply-static-deal", deal.key);
    }
}