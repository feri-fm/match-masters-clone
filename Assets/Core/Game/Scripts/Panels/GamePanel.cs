using MMC.Network.GameMiddleware;

namespace MMC.Game
{
    public class GamePanel : BasePanel
    {
        public TextMember status;

        public override void Update()
        {
            base.Update();
            if (status.value.gameObject.activeSelf)
            {
                status.text = "";
                if (network.game.client.game != null)
                {
                    NetGame netGame;
                    var serverStatus = "";
                    if (network.game.server.games.Count > 0)
                    {
                        netGame = network.game.server.games[0];
                        serverStatus = netGame.gameplay.GetChecksum();
                        status.text += "\n === server";
                        status.text += $"\n{serverStatus}";
                        status.text += $"\n{netGame.gameplay.GetHash()}";
                    }

                    netGame = network.game.client.game;
                    var clientStatus = netGame.gameplay.GetChecksum();
                    status.text += "\n === client";
                    if (serverStatus == "")
                        status.text += $"\n{clientStatus}";
                    else
                        status.text += $"\n{Diff(clientStatus, serverStatus)}";
                    status.text += $"\n{netGame.gameplay.GetHash()}";
                }
                else
                {
                    status.text = "...";
                }
            }
        }

        public string Diff(string text, string original)
        {
            var res = "";
            var isOpen = false;
            for (int i = 0; i < text.Length; i++)
            {
                var valid = i < original.Length && original[i] == text[i];
                if (valid)
                {
                    if (isOpen) res += "</b></color>";
                    isOpen = false;
                }
                else
                {
                    if (!isOpen) res += "<color=red><b>";
                    isOpen = true;
                }
                res += text[i];
            }
            if (isOpen) res += "</b></color>";
            return res;
        }

        [Member]
        public void Leave()
        {
            network.game.client.LeaveGame();
        }

        [Member]
        public void RequestGameplay()
        {
            network.game.client.client.CmdRequestGameplayData();
        }
    }
}