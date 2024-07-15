using System.Linq;
using MMC.Network.GameMiddleware;
using UnityEngine;

namespace MMC.Game
{
    public class JoiningPanel : BasePanel
    {
        public TextMember myUsername;
        public TextMember otherUsername;

        public override void OnRender()
        {
            base.OnRender();
            myUsername.text = game.user.username;
            var players = network.game.client.roomPlayers;
            if (players != null)
            {
                if (players.Any(e => e.username != game.user.username))
                    otherUsername.text = players.First(e => e.username != game.user.username).username;
                else
                    otherUsername.text = "finding...";
            }
            else
            {
                otherUsername.text = "animation...";
            }
        }

        public async void Setup(NetConfig config)
        {
            network.game.client.roomPlayers = null;
            await new WaitForSeconds(0.5f);
            network.game.client.JoinGame(config);
        }

        [Member]
        public void Leave()
        {
            network.game.client.LeaveRoom();
        }
    }
}