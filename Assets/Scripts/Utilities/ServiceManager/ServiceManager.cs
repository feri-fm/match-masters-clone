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
        instance = this;
        config.Init();
    }

    public T GetService<T>() where T : ServiceDriver
    {
        return config.GetService<T>();
    }
}
