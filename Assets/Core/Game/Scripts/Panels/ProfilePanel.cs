namespace MMC.Game
{
    public class ProfilePanel : BasePanel
    {
        public Member<InputFieldHelper> usernameInput;
        public TextMember username;
        public TextMember trophies;

        public override void OnRender()
        {
            base.OnRender();
            username.text = game.user.username;
            trophies.text = game.user.trophies.ToString();
        }

        public override void OnOpen()
        {
            base.OnOpen();
            usernameInput.value.SetTextWithoutNotify(game.user.username);
        }

        [Member]
        public void Logout()
        {
            Popup.ShowConfirmation("Are you sure to logout?", () =>
            {
                game.Logout();
            });
        }

        [Member]
        public void Back()
        {
            ClosePanel();
        }

        [Member]
        public void ChangeUsername()
        {
            game.networkManager.menu.client.ChangeUsername(usernameInput.value.text);
        }

        [Member]
        public void IncreaseTrophies()
        {
            game.networkManager.menu.client.SetTrophies(game.user.trophies + 25);
        }
        [Member]
        public void DecreaseTrophies()
        {
            game.networkManager.menu.client.SetTrophies(game.user.trophies - 25);
        }
    }
}