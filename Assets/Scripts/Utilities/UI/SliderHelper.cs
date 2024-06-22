using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderHelper : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Slider slider;

    public float _value;

    public UnityEvent<float> onValueChanged;
    public UnityEvent onStartEdit;
    public UnityEvent onEndEdit;

    private bool editStarted;

    public float value
    {
        get => _value;
        set => SetValue(value);
    }

    private void Awake()
    {
        slider.onValueChanged.AddListener((v) =>
        {
            _value = v;
            onValueChanged.Invoke(v);
        });
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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (slider.interactable)
        {
            editStarted = true;
            onStartEdit.Invoke();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (editStarted)
        {
            onEndEdit.Invoke();
        }
        editStarted = false;
    }
}
