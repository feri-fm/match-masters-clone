using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleHelper : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Toggle toggle;

    public bool _isOn;

    public UnityEvent<bool> onValueChanged;
    public UnityEvent onStartEdit;
    public UnityEvent onEndEdit;

    private bool editStarted;

    public bool isOn
    {
        get => toggle.isOn;
        set => SetValue(value);
    }

    private void Awake()
    {
        toggle.onValueChanged.AddListener((v) =>
        {
            _isOn = v;
            onValueChanged.Invoke(v);
        });
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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (toggle.interactable)
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
