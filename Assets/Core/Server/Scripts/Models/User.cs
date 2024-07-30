using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWT.Builder;
using MMC.Game;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MMC.Server.Models
{
    public class UserModel : Model<UserModel>
    {
        public string username;
        public List<string> selectedItems = new();
        public InventoryModel inventory = new();
        public InventoryModel rewards = new();
        public DailyDealsModel dailyDeals = new();
        public Dictionary<string, DateTime> timedDeals = new();

        public int trophies => inventory.GetCount("trophy");
        public int coins => inventory.GetCount("coin");

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

        public void ApplyRewards()
        {
            inventory.AddItems(rewards);
            rewards.Clear();
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

        public async Task<bool> TryApplyDailyDeal(Deal deal)
        {
            if (DateTime.Now > dailyDeals.expires)
                return false;

            if (!dailyDeals.deals.ContainsKey(deal.key))
                return false;

            var count = dailyDeals.deals[deal.key];
            if (count <= 0)
                return false;

            // dailyDeals.deals[deal.key] -= 1; //TODO: this should be done with Inc

            return await TryApplyDeal(deal, e => e.Inc(m => m.dailyDeals.deals[deal.key], -1));
        }

        public bool CanApplyTimedDeal(TimedDeal timedDeal)
        {
            var now = DateTime.Now;
            if (timedDeals.TryGetValue(timedDeal.deal.key, out var lastTime))
            {
                var delta = now - lastTime;
                if (delta.TotalSeconds <= timedDeal.time)
                    return false;
            }
            return true;
        }

        public async Task<bool> TryApplyTimedDeal(TimedDeal timedDeal)
        {
            if (!CanApplyTimedDeal(timedDeal)) return false;

            return await TryApplyDeal(timedDeal.deal, e => e.Set(m => m.timedDeals, timedDeals));
        }

        public Task<bool> TryApplyDeal(Deal deal) => TryApplyDeal(deal, b => b);
        public async Task<bool> TryApplyDeal(Deal deal, Func<UpdateDefinition<UserModel>, UpdateDefinition<UserModel>> builder)
        {
            if (deal.isPaid && inventory.GetCount(deal.price.item.key) < deal.price.count)
                return false;

            if (deal.isPaid)
                inventory.ChangeCount(deal.price.item.key, -deal.price.count);
            foreach (var reward in deal.rewards)
                rewards.ChangeCount(reward.item.key, reward.count);

            await Update(e => builder(
                e.Set(m => m.rewards, rewards)
                .Set(m => m.inventory, inventory)));

            return true;
        }
    }

    public class InventoryModel
    {
        public Dictionary<string, int> items = new();

        public int this[string key]
        {
            get => items[key];
            set => items[key] = value;
        }

        public int GetCount(string key)
        {
            if (items.TryGetValue(key, out var count))
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
            return items.ContainsKey(key);
        }
        public void UnlockItem(string key)
        {
            if (!items.ContainsKey(key))
                items.Add(key, 0);
        }
        public void SetCount(string key, int count)
        {
            items[key] = count;
        }
        public void ChangeCount(string key, int count)
        {
            if (items.ContainsKey(key))
                items[key] += count;
            else
                items[key] = count;
        }

        public void Clear()
        {
            items.Clear();
        }

        public void AddItems(InventoryModel other) => AddItems(other.items);
        public void AddItems(Dictionary<string, int> other)
        {
            foreach (var item in other)
            {
                ChangeCount(item.Key, item.Value);
            }
        }
        public void AddItems(Dictionary<Item, int> other)
        {
            foreach (var item in other)
            {
                ChangeCount(item.Key.key, item.Value);
            }
        }
    }

    public class DailyDealsModel
    {
        public Dictionary<string, int> deals = new();
        public DateTime expires;
    }
}