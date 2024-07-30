using System.Collections.Generic;
using UnityEngine;

namespace MMC.Game
{
    [CreateAssetMenu(fileName = "ShopConfig", menuName = "Game/ShopConfig")]
    public class ShopConfig : ScriptableObject
    {
        public Deal[] staticDeals;
        public TimedDeal[] timedDeals;
        public DailyDeals[] dailyDeals;

        public DailyDeals GetDailyDeals(int trophy)
        {
            for (int i = dailyDeals.Length - 1; i >= 0; i--)
                if (trophy >= dailyDeals[i].trophy)
                    return dailyDeals[i];
            return null;
        }

        public Deal GetStaticDeal(string key)
        {
            for (int i = 0; i < staticDeals.Length; i++)
                if (staticDeals[i].key == key)
                    return staticDeals[i];
            return null;
        }
        public TimedDeal GetTimedDeal(string key)
        {
            for (int i = 0; i < timedDeals.Length; i++)
                if (timedDeals[i].key == key)
                    return timedDeals[i];
            return null;
        }
    }

    [System.Serializable]
    public class TimedDeal
    {
        public string key => deal.key;

        public long time; // in seconds
        public Deal deal;
    }

    [System.Serializable]
    public class DailyDeals
    {
        public int trophy;
        public int pick;
        public DailyDeal[] deals;
    }

    [System.Serializable]
    public class DailyDeal
    {
        public int count;
        public Deal deal;
    }
}