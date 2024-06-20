using System.Collections;
using System.Collections.Generic;
using Match3;
using UnityEngine;

public class LifetimeEffect : PoolObject
{
    public TileColor color;
    public float time = 0.8f;

    private float startTime;

    protected override void OnSpawned()
    {
        base.OnSpawned();
        startTime = Time.time;
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (Time.time > startTime + time)
        {
            Pool();
        }
    }
}
