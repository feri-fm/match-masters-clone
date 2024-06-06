using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleHelper : MonoBehaviour
{
    public Toggle toggle;

    public bool _isOn;

    public bool isOn
    {
        get => toggle.isOn;
        set => SetValue(value);
    }

    public void SetValue(bool isOn)
    {
        _isOn = isOn;
        toggle.isOn = isOn;
    }

    public void SetIsOnWithoutNotify(bool isOn)
    {
        _isOn = isOn;
        toggle.SetIsOnWithoutNotify(isOn);
    }
}
