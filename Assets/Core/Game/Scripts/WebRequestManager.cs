using MMC.Server.Models;
using UnityEngine;
using WebClient;

namespace MMC.Game
{
    public class WebRequestManager : MonoBehaviour
    {
        public WebRequestBuilder requestBuilder;

        public string token;
        public bool hasToken;
        public bool isLoggedIn;

        public GameManager game => GameManager.instance;

        public string tokenKey => game.gameService.dataKey + "_token";

        private void Start()
        {
            requestBuilder = new WebRequestBuilder();
            requestBuilder.config = game.gameService.webConfig;

            if (PlayerPrefs.HasKey(tokenKey))
            {
                token = PlayerPrefs.GetString(tokenKey);
                hasToken = true;
            }
            else
            {
                hasToken = false;
            }

            requestBuilder.onRequestCreated += (req) =>
            {
                req.AddHeader("x-auth-token", token);

                req.R(r =>
                {
                    game.ChangeState();
                });
                req.F(r =>
                {
                    if (r.status == 401 || r.status == 404)
                    {
                        Logout();
                        game.Startup();
                    }
                    game.ChangeState();

                    Popup.ShowAlert(r.error ?? r.body);
                });
            };
        }

        public Request Register() => requestBuilder.Post("/api/auth/register").R(r =>
        {
            token = r.GetResponseHeader("x-auth-token");
            PlayerPrefs.SetString(tokenKey, token);
            hasToken = true;
            isLoggedIn = true;
            game.user = r.GetBody<UserModel>();
            game.ChangeState();
        });

        public Request Validate() => requestBuilder.Get("/api/auth/validate").R(r =>
        {
            isLoggedIn = true;
            game.user = r.GetBody<UserModel>();
            game.ChangeState();
        });

        public void Logout()
        {
            PlayerPrefs.DeleteKey(tokenKey);
            token = "";
            isLoggedIn = false;
            hasToken = false;
            game.user = new UserModel();
            game.ChangeState();
        }

        public Request GetNetworkConnection() => requestBuilder.Get("/api/network/request-connection").R(r =>
        {

        });
    }
}