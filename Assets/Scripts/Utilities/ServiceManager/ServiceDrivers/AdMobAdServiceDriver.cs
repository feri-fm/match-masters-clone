using System.Collections;
using System.Collections.Generic;
// using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "AdMobAd", menuName = "Services/AdMobAd")]
public class AdMobAdServiceDriver : AdServiceDriver
{
    public string defaultKey = "default";
    public List<AdMobKey> keys = new List<AdMobKey>();

    public Dictionary<string, ServiceAd> ads = new Dictionary<string, ServiceAd>();
    // public Dictionary<string, RewardedAd> myAds = new Dictionary<string, RewardedAd>();

    public override void Init()
    {
        base.Init();
        // MobileAds.Initialize(initStatus =>
        // {
        //     foreach (var key in keys)
        //     {
        //         if (key.autoLoad)
        //         {
        //             LoadRewarded(key.key, (_) => { });
        //         }
        //     }
        // });
    }

    public AdMobKey GetAdKey(string key)
    {
        var item = keys.Find(e => e.key == key);
        if (item == null) return keys.Find(e => e.key == defaultKey);
        return item;
    }
    public string GetUnitId(string key)
    {
        return GetUnitId(GetAdKey(key));
    }
    public string GetUnitId(AdMobKey key)
    {
#if UNITY_ANDROID
        return key.android;
#elif UNITY_IPHONE
        return key.ios;
#else
        return "unexpected_platform";
#endif
    }

    public override void LoadRewarded(string key, UnityAction<ServiceAd> callback)
    {
        base.LoadRewarded(key, callback);
        // if (ads.TryGetValue(key, out var oldAd))
        //     oldAd.Destroy();
        // if (myAds.TryGetValue(key, out var oldMyAd))
        //     oldMyAd.Destroy();

        // var adKey = GetAdKey(key);
        // var unitId = GetUnitId(adKey);
        // key = adKey.key;
        // var myAd = new RewardedAd(unitId);
        // var ad = new ServiceAd(key);
        // myAd.OnAdLoaded += (sender, args) =>
        // {

        // };
        // myAd.OnUserEarnedReward += (sender, args) =>
        // {
        //     ad.onRewarded.Invoke();
        // };
        // myAd.OnAdClosed += (sender, args) =>
        // {
        //     ad.onClosed.Invoke();
        //     if (adKey.autoLoad)
        //         LoadRewarded(key, (_) => { });
        // };
        // myAd.OnAdFailedToLoad += (sender, args) =>
        // {
        //     ad.onFailedLoad.Invoke();
        //     if (adKey.autoLoad)
        //         LoadRewarded(key, (_) => { });
        // };
        // ads[key] = ad;
        // myAds[key] = myAd;

        // AdRequest request = new AdRequest.Builder().Build();
        // myAd.LoadAd(request);
    }

    public override void ShowRewarded(string key, UnityAction rewarded)
    {
        base.ShowRewarded(key, rewarded);
        // var adKey = GetAdKey(key);
        // key = adKey.key;
        // if (ads.TryGetValue(key, out var ad)
        //     && myAds.TryGetValue(key, out var myAd))
        // {
        //     if (myAd.IsLoaded())
        //     {
        //         ad.onRewarded = () =>
        //         {
        //             rewarded();
        //         };
        //         myAd.Show();
        //     }
        // }
    }

    [System.Serializable]
    public class AdMobKey
    {
        public string key;
        public bool autoLoad = true;
        public string android;
        public string ios;
    }
}
