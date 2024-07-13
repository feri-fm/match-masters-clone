using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EditorAd", menuName = "Services/EditorAd")]
public class EditorAdServiceDriver : AdServiceDriver
{
    public bool enabled;

    public override void ShowRewarded(string key, UnityAction rewarded)
    {
        base.ShowRewarded(key, rewarded);
        if (enabled)
        {
            var manager = EditorAdManager.instance;
            if (manager != null)
            {
                manager.Show();
                manager.reward = () =>
                {
                    rewarded.Invoke();
                };
                manager.error = () =>
                {

                };
            }
            else
            {
                rewarded.Invoke();
            }
        }
        else
        {
            rewarded.Invoke();
        }
    }
}
