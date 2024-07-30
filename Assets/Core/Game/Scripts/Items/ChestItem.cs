using System.Collections.Generic;
using UnityEngine;

namespace MMC.Game
{
    public class ChestItem : Item
    {
        public WeightItem[] items;
        public Vector2Int count;

        public Dictionary<Item, int> EvaluateItems()
        {
            var res = new Dictionary<Item, int>();
            var left = new List<WeightItem>(items);
            var c = Mathf.Min(count.Random(), items.Length);
            for (int i = 0; i < c; i++)
            {
                var item = EvaluateItem(left);
                left.Remove(item);
                if (res.ContainsKey(item.item))
                    res[item.item] += item.count;
                else
                    res[item.item] = item.count;
            }
            return res;
        }

        private WeightItem EvaluateItem(List<WeightItem> items)
        {
            int index = 0;

            float totalWeight = 0;
            foreach (var item in items)
                totalWeight += item.weight;

            float rand = Random.Range(0f, totalWeight);
            for (int i = 0; i < items.Count; i++)
            {
                if (rand < items[i].weight)
                {
                    index = i;
                    break;
                }

                rand -= items[i].weight;
            }

            return items[index];
        }

        public class WeightItem
        {
            public Item item;
            public int count = 1;
            public float weight = 1;
        }
    }
}