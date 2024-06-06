using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public bool pooled { get; private set; } = false;
    public bool active { get; private set; } = false;
    public object poolKey { get; private set; } = null;
    public ObjectPool pool { get; private set; } = null;
    public PoolObject prefab { get; private set; } = null;

    private bool created = false;

    public virtual void Awake() { }
    public virtual void Start()
    {
        if (!created)
        {
            created = true;
            OnCreated();
            OnSpawned();
        }
    }

    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void LateUpdate() { }

    public virtual void OnCreated() { }
    public virtual void OnSpawned() { }
    public virtual void OnPooled() { }
    public virtual void OnDestroyed() { }

    public void _OnCreated(object poolKey, ObjectPool pool, PoolObject prefab)
    {
        this.poolKey = poolKey;
        this.pool = pool;
        this.prefab = prefab;
        created = true;
        OnCreated();
    }
    public void _OnSpawned()
    {
        pooled = false;
        OnSpawned();
    }

    public void _OnPooled() { OnPooled(); }
    public void _OnDestroyed() { OnDestroyed(); }

    public void Pool()
    {
        if (!pooled)
        {
            pool?.Pool(poolKey, this);
            pooled = true;
        }
    }

    public void Destroy()
    {
        pool?.Destroy(poolKey, this);
        _OnDestroyed();
        Destroy(gameObject);
    }

    public void SetActive(bool active)
    {
        this.active = active;
        gameObject.SetActive(active);
    }
}
