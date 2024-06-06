using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ObjectPool))]
public class ListLoader : MonoBehaviour
{
    public ListItem prefab;
    public Transform container;

    public ObjectPool pool => _pool ?? (_pool = GetComponent<ObjectPool>()); ObjectPool _pool;

    public List<ListItem> currentItems { get; } = new List<ListItem>();

    private void Awake()
    {
        prefab.SetActive(false);
    }

    public void Setup(IEnumerable items)
    {
        Clear();
        var index = 0;
        foreach (var data in items)
        {
            var item = pool.Spawn(prefab, container);
            item.transform.SetAsLastSibling();
            currentItems.Add(item);
            item._Setup(this, data, index);
            index++;
        }
    }

    public void Clear()
    {
        foreach (var item in currentItems)
            item.Pool();
        currentItems.Clear();
    }
}
