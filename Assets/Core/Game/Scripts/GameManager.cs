using System;
using MMC.Network;
using MMC.Server.Models;
using UnityEngine;

namespace MMC.Game
{
    public class GameManager : MonoBehaviour
    {
        public GameConfig config;
        public PanelGroup panelGroup;
        public CameraController cameraController;
        public ServiceManager serviceManager;
        public WebRequestManager webRequestManager;
        public NetNetworkManager networkManager;

        public StartupPanel startupPanel => panelGroup.GetPanel<StartupPanel>();
        public ConnectingPanel connectingPanel => panelGroup.GetPanel<ConnectingPanel>();
        public MenuPanel menuPanel => panelGroup.GetPanel<MenuPanel>();
        public AuthPanel authPanel => panelGroup.GetPanel<AuthPanel>();
        public ProfilePanel profilePanel => panelGroup.GetPanel<ProfilePanel>();
        public JoiningPanel joiningPanel => panelGroup.GetPanel<JoiningPanel>();
        public GamePanel gamePanel => panelGroup.GetPanel<GamePanel>();
        public FinishGamePanel finishGamePanel => panelGroup.GetPanel<FinishGamePanel>();
        public BoostersPanel boostersPanel => panelGroup.GetPanel<BoostersPanel>();

        public GameServiceDriver gameService => serviceManager.GetService<GameServiceDriver>();

        public event Action onStateChanged = delegate { };

        public UserModel user;
        public bool isConnecting;
        public bool isConnected;

        public GameData data;

        private bool dirtyData;

        public static GameManager instance { get; private set; }

        private void Awake()
        {
            instance = this;
            networkManager.transport.OnClientDisconnected += () =>
            {
                OnDisconnect();
            };

            Application.targetFrameRate = 60;
        }

        private async void Start()
        {
            if (PlayerPrefs.HasKey(gameService.dataKey))
            {
                data = PlayerPrefs.GetString(gameService.dataKey).FromJson<GameData>();
            }

            await 0.1f;
            Startup();
        }

        //TODO: these are not good
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
        public void Login(string username)
        {
            ChangeState(() => isConnecting = true);
            webRequestManager.Login(username).R(r =>
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
            ChangeState(() => isConnecting = false);
            ChangeState(() => isConnected = true);
            connectingPanel.ClosePanel();
        }

        public Chapter GetCurrentChapter()
        {
            return config.GetChapter(user.trophies);
        }

        public void Play()
        {
            boostersPanel.OpenPanel();
        }

        public void Join()
        {
            var chapter = GetCurrentChapter();
            var config = networkManager.game.GetConfig(chapter.key);
            joiningPanel.Setup(config);
            joiningPanel.OpenPanel();
        }

        public void Logout()
        {
            ChangeState(() => isConnecting = false);
            ChangeState(() => isConnected = false);
            webRequestManager.Logout();
            networkManager.StopClient();
            Startup();
        }

        private void Update()
        {

        }

        private void LateUpdate()
        {
            if (dirtyData)
            {
                dirtyData = false;
                PlayerPrefs.SetString(gameService.dataKey, data.ToJson());
            }
        }

        public void ChangeState() => ChangeState(() => { });
        public void ChangeState(Action action)
        {
            action.Invoke();
            onStateChanged.Invoke();
        }

        public void ChangeData(Action action) => ChangeData((_) => action.Invoke());
        public void ChangeData(Action<GameData> action)
        {
            action.Invoke(data);
            dirtyData = true;
            ChangeState();
        }
    }

    public class GameData
    {

    }
}