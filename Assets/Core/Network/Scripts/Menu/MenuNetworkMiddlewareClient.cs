using System;
using System.Collections.Generic;
using Mirror;
using MMC.Game;
using MMC.Server.Models;
using Newtonsoft.Json.Linq;

namespace MMC.Network.MenuMiddleware
{
    public class MenuNetworkMiddlewareClient : NetNetworkMiddlewareClient<MenuNetworkMiddleware>
    {
        public override void Setup()
        {
            base.Setup();
            On<UserModel>("update-user", user =>
            {
                gameManager.ChangeState(() =>
                {
                    gameManager.user = user;
                });
            });
        }

        public void UpdateSelectedItems(string[] items)
        {
            Emit("update-selected-items", items);
        }
        public void UnlockBooster(Booster booster) //TODO: this is wrong
        {
            Emit("unlock-booster", booster.key);
        }
        public void SetItemCount(string key, int count) //TODO: this is wrong
        {
            Emit("set-item-count", $"{key}:{count}");
        }
    }
}