using System.Collections.Generic;
using UnityEngine;

namespace MMC.Game
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public Booster defaultBooster;
        public List<Perk> defaultPerks;

        public List<Booster> boosters = new();
        public List<Perk> perks = new();
        public List<Chapter> chapters = new();

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

        public Chapter GetChapter(int trophies)
        {
            for (int i = chapters.Count - 1; i >= 0; i--)
            {
                if (trophies >= chapters[i].trophy)
                {
                    return chapters[i];
                }
            }
            return null;
        }
    }
}