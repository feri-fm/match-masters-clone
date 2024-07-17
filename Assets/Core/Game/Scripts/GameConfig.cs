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
    }
}