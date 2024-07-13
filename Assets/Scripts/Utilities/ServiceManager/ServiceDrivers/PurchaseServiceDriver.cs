using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PurchaseServiceDriver : ServiceDriver
{
    public virtual void Purchase(string key, UnityAction<ServicePurchase> callback)
    {

    }

    public virtual void GetProduct(string key, UnityAction<ServiceProduct> callback)
    {

    }
}

public class ServicePurchase
{
    public Guid id;

    public UnityAction onSucceed = () => { };
    public UnityAction onFailure = () => { };

    public ServiceProduct product;

    public ServicePurchase(ServiceProduct product)
    {
        this.product = product;
        id = Guid.NewGuid();
    }
}

public class ServiceProduct
{
    public string key;

    public decimal price;
    public string priceText;

    public ServiceProduct(string key)
    {
        this.key = key;
    }
}
