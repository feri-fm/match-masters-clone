using UnityEngine;

namespace WebClient
{
    public class WebClientManager : MonoBehaviour
    {
        public static WebClientManager instance => _instance ?? _Load();
        private static WebClientManager _instance;

        private static WebClientManager _Load()
        {
            if (_instance == null)
            {
                var obj = new GameObject("WebClientManager");
                _instance = obj.AddComponent<WebClientManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
}