using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using MMC.Network.SessionMiddleware;
using MMC.Server;
using MMC.Server.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json.Linq;

namespace MMC.Network.MenuMiddleware
{
    public class MenuNetworkMiddlewareServer : NetNetworkMiddlewareServer<MenuNetworkMiddleware>
    {
        public override void Setup()
        {
            base.Setup();
            var users = serverManager.app.Find<UsersService>();

            On<string[]>("update-selected-items", async (session, selectedItems) =>
            {
                var user = session.user;
                if (user.inventory.HasItems(selectedItems))
                {
                    user.selectedItems = selectedItems.ToList();
                    await user.Update(e => e.Set(u => u.selectedItems, user.selectedItems));
                }
                Emit(session, "update-user", user);
            });
            On<string>("unlock-booster", async (session, key) =>
            {
                var user = session.user;
                if (!user.inventory.HasItem(key))
                {
                    user.inventory.AddItem(key);
                    user.inventory.SetCount(key, 1);
                    await user.Update(e => e.Set(u => u.inventory, user.inventory));
                }
                Emit(session, "update-user", user);
            });
            On<string>("set-item-count", async (session, row) => // row => key:count
            {
                var key = row.Split(":")[0];
                var count = int.Parse(row.Split(":")[1]);
                var user = session.user;
                if (user.inventory.HasItem(key))
                {
                    user.inventory.SetCount(key, count);
                    await user.Update(e => e.Set(u => u.inventory, user.inventory));
                }
                Emit(session, "update-user", user);
            });
            On<int>("set-trophies", async (session, trophies) =>
            {
                var user = session.user;
                user.inventory.SetCount("trophies", trophies);
                await user.Update(e => e.Set(u => u.inventory, user.inventory));
                Emit(session, "update-user", user);
            });
            On<string>("change-username", async (session, username) =>
            {
                username = username.Trim();
                if (username == "")
                {
                    Emit(session, "message", "Username cannot be empty");
                    return;
                }
                var other = await users.FindUser(username);
                if (other != null)
                {
                    Emit(session, "message", "Username " + username + " is not available");
                    return;
                }
                var user = session.user;
                user.username = username;
                await user.Update(e => e.Set(u => u.username, user.username));
                Emit(session, "update-user", user);
            });
        }
    }
}