using System;
using MMC.Network;
using MMC.Server.Models;
using UnityEngine;

namespace MMC.Game
{
    public class GameManager : MonoBehaviour
    {
        public PanelGroup panelGroup;
        public WebRequestManager webRequestManager;
        public NetNetworkManager networkManager;

        public StartupPanel startupPanel => panelGroup.GetPanel<StartupPanel>();
        public ConnectingPanel connectingPanel => panelGroup.GetPanel<ConnectingPanel>();
        public MenuPanel menuPanel => panelGroup.GetPanel<MenuPanel>();
        public AuthPanel authPanel => panelGroup.GetPanel<AuthPanel>();
        public JoiningPanel joiningPanel => panelGroup.GetPanel<JoiningPanel>();
        public GamePanel gamePanel => panelGroup.GetPanel<GamePanel>();

        public event Action onStateChanged = delegate { };

        public UserModel user;
        public bool isConnecting;
        public bool isConnected;

        public static GameManager instance { get; private set; }

        private void Awake()
        {
            instance = this;
            networkManager.transport.OnClientDisconnected += () =>
            {
                OnDisconnect();
            };
        }

        private async void Start()
        {
            await 0.1f;
            Startup();
        }

        public void Register()
        {
            ChangeState(() => isConnecting = true);
            webRequestManager.Register().R(r =>
            {
                Startup();
            }).F(r =>
            {
                ChangeState(() => isConnecting = false);
            }).Send();
        }

        public void Startup()
        {
            if (webRequestManager.isLoggedIn)
            {
                StartMenu();
            }
            else
            {
                startupPanel.OpenPanel();
                if (webRequestManager.hasToken)
                {
                    ChangeState(() => isConnecting = true);
                    webRequestManager.Validate().R(r =>
                    {
                        ChangeState(() => isConnecting = false);
                        StartMenu();
                    }).F(r =>
                    {
                        ChangeState(() => isConnecting = false);
                    }).Send();
                }
                else
                {
                    authPanel.OpenPanel();
                }
            }
        }

        public void StartMenu()
        {
            menuPanel.OpenPanel();
            if (!isConnected)
            {
                connectingPanel.OpenPanel();
                ChangeState(() => isConnecting = true);
                webRequestManager.GetNetworkConnection().R(r =>
                {
                    var connection = r.GetBody<NetworkConnectionInfo>();
                    networkManager.StartClient(connection);
                }).F(r =>
                {
                    ChangeState(() => isConnecting = false);
                }).Send();
            }
        }

        public void OnDisconnect()
        {
            Debug.Log("On disconnected");
            if (isConnected)
            {
                ChangeState(() => isConnected = false);
                StartMenu();
            }
            else
            {
                ChangeState(() => isConnecting = false);
                connectingPanel.OpenPanel();
            }
        }

        public void OnSessionCreated()
        {
            Debug.Log("Session created");
            ChangeState(() => isConnected = true);
            connectingPanel.ClosePanel();
        }

        public void Join()
        {
            var config = networkManager.game.configs[0];
            joiningPanel.Setup(config);
            joiningPanel.OpenPanel();
        }

        private void Update()
        {

        }

        private void LateUpdate()
        {

        }

        public void ChangeState() => ChangeState(() => { });
        public void ChangeState(Action action)
        {
            action.Invoke();
            onStateChanged.Invoke();
        }
    }
}