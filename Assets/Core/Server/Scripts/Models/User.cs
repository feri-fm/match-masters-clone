using System.Collections.Generic;
using System.Linq;
using JWT.Builder;
using MMC.Game;
using MongoDB.Driver;

namespace MMC.Server.Models
{
    public class UserModel : Model<UserModel>
    {
        public string username;
        public List<string> selectedItems = new();
        public InventoryModel inventory = new();

        public void BuildToken(JwtBuilder builder)
        {
            builder.AddClaim("id", id.ToString());
        }

        public bool IsSelected(string key)
        {
            return selectedItems.Contains(key);
        }
        public void SelectItem(string key)
        {
            selectedItems.Add(key);
        }
        public void UnSelectItem(string key)
        {
            selectedItems.Remove(key);
        }

        public void SelectBooster(GameConfig config, string key)
        {
            for (int i = selectedItems.Count - 1; i >= 0; i--)
            {
                if (config.HasBooster(selectedItems[i]))
                {
                    selectedItems.RemoveAt(i);
                }
            }
            selectedItems.Add(key);
        }

        public string GetBooster(GameConfig config)
        {
            return selectedItems.Find(e => config.boosters.Any(b => b.key == e)) ?? config.defaultBooster.key;
        }
        public string[] GetPerks(GameConfig config)
        {
            var perks = config.defaultPerks.Select(e => e.key).ToArray();
            var index = 0;
            for (int i = 0; i < selectedItems.Count && index < perks.Length; i++)
            {
                if (config.perks.Any(b => b.key == selectedItems[i]))
                    perks[index] = selectedItems[i];
            }
            return perks;
        }
    }

    public class InventoryModel
    {
        public List<string> items = new();
        public Dictionary<string, int> counts = new();

        public int GetCount(string key)
        {
            if (counts.TryGetValue(key, out var count))
                return count;
            return 0;
        }
        public bool HasItems(string[] keys)
        {
            foreach (var key in keys)
                if (!HasItem(key)) return false;
            return true;
        }
        public bool HasItem(string key)
        {
            return items.Contains(key);
        }
        public void AddItem(string key)
        {
            items.Add(key);
        }
        public void SetCount(string key, int count)
        {
            counts[key] = count;
        }
        public void ChangeCount(string key, int count)
        {
            counts[key] += count;
        }
    }
}