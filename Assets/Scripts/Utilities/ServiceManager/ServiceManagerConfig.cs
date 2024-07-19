using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ServiceManager", menuName = "Services/ServiceManager")]
public class ServiceManagerConfig : ScriptableObject
{
    public List<ServiceGroup> groups = new List<ServiceGroup>();

    public string selectedGroupNameEditor = "";
    public string selectedGroupNameBuild = "";

    public ServiceGroup group { get; private set; }

    public List<ServiceDriver> services { get; private set; }

    public ServiceGroup GetSelectGroup()
    {
#if UNITY_EDITOR
        return groups.Find(e => e.name == selectedGroupNameEditor);
#else
        return groups.Find(e => e.name == selectedGroupNameBuild);
#endif
    }

    public void SetGroupName(string groupName)
    {
        if (groups.Find(e => e.name == groupName) != null)
        {
            selectedGroupNameBuild = groupName;
        }
        else
        {
            throw new System.Exception("Group name does not exists!");
        }
    }
    public void SetGroupNameEditor(string groupName)
    {
        if (groups.Find(e => e.name == groupName) != null)
        {
            selectedGroupNameEditor = groupName;
        }
        else
        {
            throw new System.Exception("Group name does not exists!");
        }
    }

    public T GetService<T>() where T : ServiceDriver
    {
        return GetSelectGroup()?.services.Find(e => e as T != null) as T;
    }

    public void Init()
    {
        var platform = Application.platform;
        group = GetSelectGroup();
        services = group.services.ToList();
        foreach (var driver in services)
        {
            driver.Init();
        }
    }
}

[System.Serializable]
public class ServiceGroup
{
    public string name;
    public List<ServiceDriver> services = new List<ServiceDriver>();
}
