using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private Dictionary<object, List<PoolObject>> pools = new Dictionary<object, List<PoolObject>>();

    public static ObjectPool global => _global ?? LoadGlobal(); static ObjectPool _global;
    private static ObjectPool LoadGlobal()
    {
        if (_global == null)
        {
            var obj = new GameObject("GlobalObjectPool");
            DontDestroyOnLoad(obj);
            _global = obj.AddComponent<ObjectPool>();
        }
        return _global;
    }

    public T Spawn<T>(object key, T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : PoolObject
    {
        if (!pools.ContainsKey(key))
        {
            pools.Add(key, new List<PoolObject>());
        }

        var pool = pools[key];

        if (pool.Count > 0)
        {
            var obj = pool[0];
            pool.RemoveAt(0);
            UpdateInstance(obj, position, rotation, parent);
            obj.SetActive(true);
            obj._OnSpawned();
            return obj as T;
        }
        else
        {
            var obj = GetInstance(prefab, position, rotation, parent);
            obj.SetActive(true);
            obj._OnCreated(key, this, prefab);
            obj._OnSpawned();
            return obj as T;
        }
    }

    public void Pool(object key, PoolObject obj)
    {
        if (!pools.ContainsKey(key))
        {
            pools.Add(key, new List<PoolObject>());
        }

        var pool = pools[key];
        pool.Add(obj);
        obj.SetActive(false);
        obj._OnPooled();
    }
    public void Destroy(object key, PoolObject obj)
    {

    }

    public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : PoolObject
    {
        return Spawn(prefab, prefab, position, rotation, parent);
    }
    public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : PoolObject
    {
        return Spawn(prefab, prefab, position, rotation, null);
    }
    public T Spawn<T>(T prefab, Transform parent) where T : PoolObject
    {
        return Spawn(prefab, prefab, prefab.transform.position, prefab.transform.rotation, parent);
    }
    public T Spawn<T>(T prefab) where T : PoolObject
    {
        return Spawn(prefab, prefab, prefab.transform.position, prefab.transform.rotation, null);
    }

    public virtual T GetInstance<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : PoolObject
    {
        return Instantiate(prefab, position, rotation, parent);
    }

    public virtual void UpdateInstance<T>(T instance, Vector3 position, Quaternion rotation, Transform parent) where T : PoolObject
    {
        instance.transform.SetPositionAndRotation(position, rotation);
        instance.transform.SetParent(parent);
        // instance.transform.SetAsLastSibling();
    }
}
