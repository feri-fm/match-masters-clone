using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServiceManager : MonoBehaviour
{
    public ServiceManagerConfig config;

    public static ServiceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        config.Init();
    }

    public T GetService<T>() where T : ServiceDriver
    {
        return config.GetService<T>();
    }
}
