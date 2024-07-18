namespace MMC.Game
{
    public class GamePanel : BasePanel
    {
        public TextMember status;

        public override void Update()
        {
            base.Update();
            // status.text = "";
            // if (network.game.client.game != null)
            // {
            //     var netGame = network.game.server.games[0];
            //     var serverStatus = netGame.gameplay.GetHash();
            //     status.text += "\n === server";
            //     status.text += $"\n{serverStatus}";
            //     status.text += $"\n{netGame.gameplay.GetRealHash()}";


            //     netGame = network.game.client.game;
            //     var clientStatus = netGame.gameplay.GetHash();
            //     status.text += "\n === client";
            //     status.text += $"\n{Diff(clientStatus, serverStatus)}";
            //     status.text += $"\n{netGame.gameplay.GetRealHash()}";
            // }
            // else
            // {
            //     status.text = "...";
            // }
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