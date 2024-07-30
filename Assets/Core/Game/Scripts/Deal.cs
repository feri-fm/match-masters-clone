using UnityEngine;

namespace MMC.Game
{
    [CreateAssetMenu(fileName = "deal", menuName = "Deals/deal")]
    public class Deal : ScriptableObject
    {
        public string key => name;

        public string prefix = "deal";
        public DealItem price;
        public DealItem[] rewards;

        public bool isFree => price?.item == null;
        public bool isPaid => !isFree;

        public string GetName()
        {
            var res = price.item != null
                ? $"{prefix}_{price.item.key}_{price.count},"
                : $"{prefix}_free,";
            for (int i = 0; i < rewards.Length; i++)
            {
                var reward = rewards[i];
                res += $"{reward.item.key}_{reward.count}";
                if (i < rewards.Length - 1)
                    res += ",";
            }
            return res;
        }
    }

    [System.Serializable]
    public class DealItem
    {
        public Item item;
        public int count;

        public string GetText()
        {
            return $"{item.key} x {count}";
        }
    }
}