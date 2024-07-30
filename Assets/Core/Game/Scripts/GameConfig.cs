using System.Collections.Generic;
using UnityEngine;

namespace MMC.Game
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public ShopConfig shop;

        public Booster defaultBooster;
        public List<Perk> defaultPerks;

        public List<Booster> boosters = new();
        public List<Perk> perks = new();
        public List<Chapter> chapters = new();
        public List<Item> items = new();
        public List<Deal> deals = new();

        public Booster GetBooster(string key)
        {
            for (int i = 0; i < boosters.Count; i++)
                if (boosters[i].key == key) return boosters[i];
            return null;
        }
        public bool HasBooster(string key)
        {
            for (int i = 0; i < boosters.Count; i++)
                if (boosters[i].key == key) return true;
            return false;
        }

        public Perk GetPerk(string key)
        {
            for (int i = 0; i < perks.Count; i++)
                if (perks[i].key == key) return perks[i];
            return null;
        }
        public bool HasPerk(string key)
        {
            for (int i = 0; i < perks.Count; i++)
                if (perks[i].key == key) return true;
            return false;
        }

        public Chapter GetChapter(string key)
        {
            for (int i = 0; i < chapters.Count; i++)
                if (chapters[i].key == key) return chapters[i];
            return null;
        }

        public Chapter GetChapter(int trophy)
        {
            for (int i = chapters.Count - 1; i >= 0; i--)
                if (trophy >= chapters[i].trophy)
                    return chapters[i];
            return null;
        }

        public Item GetItem(string key)
        {
            for (int i = 0; i < items.Count; i++)
                if (items[i].key == key) return items[i];
            return null;
        }

        public Deal GetDeal(string key)
        {
            for (int i = 0; i < deals.Count; i++)
                if (deals[i].key == key)
                    return deals[i];
            return null;
        }
    }
}