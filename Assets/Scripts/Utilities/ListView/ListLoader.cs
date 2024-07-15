using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class ListLoader : MonoBehaviour
{
    public Transform container;
    public ListItem prefab;
    public bool notScenePrefab;

    public readonly List<ListItem> currentItems = new();

    public ObjectPool pool => _pool ?? (_pool = GetComponent<ObjectPool>()); ObjectPool _pool;

    private void Awake()
    {
        if (!notScenePrefab)
        {
            prefab.SetActive(false);
        }
    }

    public void Clear()
    {
        foreach (var item in currentItems)
        {
            Destroy(item.gameObject);
        }

        currentItems.Clear();
    }

    public void UpdateItems(IEnumerable items)
    {
        var leftItems = currentItems.ToList();
        var index = 0;
        foreach (var item in items)
        {
            var view = leftItems.Find(e => e.IsEqual(e.data));
            if (view == null)
            {
                view = pool.Spawn(prefab, container);
                view.gameObject.SetActive(true);
                view._SetupLoader(this);
                currentItems.Add(view);
            }
            else
            {
                leftItems.Remove(view);
            }

            view.transform.SetSiblingIndex(index);
            index++;
            view._SetupData(item);
        }

        foreach (var item in leftItems)
        {
            currentItems.Remove(item);
            item.Pool();
        }
    }
}