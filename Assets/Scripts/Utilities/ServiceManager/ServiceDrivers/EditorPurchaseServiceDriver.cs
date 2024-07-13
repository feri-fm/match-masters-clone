using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EditorPurchase", menuName = "Services/EditorPurchase")]
public class EditorPurchaseServiceDriver : PurchaseServiceDriver
{
    public override void Purchase(string key, UnityAction<ServicePurchase> callback)
    {
        base.Purchase(key, callback);
        var purchase = new ServicePurchase(new ServiceProduct("key"));
        callback.Invoke(purchase);
        purchase.onSucceed.Invoke();
    }

    public override void GetProduct(string key, UnityAction<ServiceProduct> callback)
    {
        base.GetProduct(key, callback);
        callback.Invoke(new ServiceProduct(key)
        {
            price = 0,
            priceText = "0.00$"
        });
    }
}
