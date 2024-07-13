using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AdServiceDriver : ServiceDriver
{
    public virtual void LoadRewarded(string key, UnityAction<ServiceAd> callback)
    {
        var dic = new Dictionary<string, int>()
        {
            { "key", key.GetHashCode() }
        };
    }

    public virtual void ShowRewarded(string key, UnityAction rewarded)
    {

    }
}

public class ServiceAd
{
    public string key;
    public UnityAction onRewarded = () => { };
    public UnityAction onClosed = () => { };
    public UnityAction onFailedLoad = () => { };

    public ServiceAd(string key)
    {
        this.key = key;
    }

    public void Destroy()
    {
        onRewarded = () => { };
        onClosed = () => { };
        onFailedLoad = () => { };
    }
}