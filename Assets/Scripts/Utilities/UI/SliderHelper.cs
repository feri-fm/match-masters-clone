using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderHelper : MonoBehaviour
{
    public Slider slider;

    public float _value;

    public float value
    {
        get => _value;
        set => SetValue(value);
    }

    public void SetValue(float value)
    {
        _value = value;
        slider.value = value;
    }

    public void SetValueWithoutNotify(float value)
    {
        _value = value;
        slider.SetValueWithoutNotify(value);
    }
}
