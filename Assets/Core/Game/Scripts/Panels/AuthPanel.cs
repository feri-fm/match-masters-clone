using UnityEngine;

namespace MMC.Game
{
    public class AuthPanel : BasePanel
    {
        public Member<InputFieldHelper> username;
        public GameObjectMember connecting;

        public override void OnOpen()
        {
            base.OnOpen();
            username.value.SetTextWithoutNotify(PlayerPrefs.GetString("_auth_username"));
        }

        public override void OnRender()
        {
            base.OnRender();
            connecting.SetActive(game.isConnecting);
        }

        [Member]
        public void Register()
        {
            game.Register();
        }

        [Member]
        public void Login()
        {
            PlayerPrefs.SetString("_auth_username", username.value.text);
            game.Login(username.value.text);
        }
    }
}