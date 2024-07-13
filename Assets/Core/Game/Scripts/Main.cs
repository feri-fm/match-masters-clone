using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    private void Start()
    {
#if UNITY_SERVER
        SceneManager.LoadScene("Server");
#else
        SceneManager.LoadScene("Game");
#endif
    }
}
