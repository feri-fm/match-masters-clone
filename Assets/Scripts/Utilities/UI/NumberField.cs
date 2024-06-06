using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class NumberField : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public InputFieldHelper inputField;

    public float _value;
    public float step = 0.1f;
    public float speed = 1;
    public string format = "0.0";

    public UnityEvent<float> onValueChanged;
    public UnityEvent onStartEdit;
    public UnityEvent onEndEdit;

    public float value
    {
        get => _value;
        set => SetValue(value);
    }

    private Vector2 startPosition;
    private float startValue;

    private float preEditValue;

    private bool editing;
    private bool startedEditing;

    private void Awake()
    {
        startedEditing = false;
        inputField.onStartEdit.AddListener(() =>
        {
            preEditValue = _value;
            onStartEdit.Invoke();
            startedEditing = true;
        });
        inputField.inputField.onEndEdit.AddListener((txt) =>
        {
            if (float.TryParse(txt, out var num))
            {
                SetValue(num);
            }
            else
            {
                SetValueWithoutNotify(preEditValue);
            }
            if (startedEditing)
            {
                startedEditing = false;
                onEndEdit.Invoke();
            }
        });
        SetValueWithoutNotify(_value);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = eventData.position;
        startValue = _value;
        editing = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var delta = ((eventData.position.x - startPosition.x) + (eventData.position.y - startPosition.y) / 10) * speed;
        if (step > 0)
            delta = Mathf.Round(delta / step) * step;
        var newValue = startValue + delta;
        if (newValue != value)
        {
            if (!editing)
            {
                editing = true;
                onStartEdit.Invoke();
            }
            SetValue(newValue);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SetValue(_value);
        if (editing)
        {
            editing = false;
            onEndEdit.Invoke();
        }
    }

    public void SetValue(float value)
    {
        _value = value;
        inputField.text = GetTextValue();
        onValueChanged.Invoke(value);
    }
    public void SetValueWithoutNotify(float value)
    {
        _value = value;
        inputField.SetTextWithoutNotify(GetTextValue());
    }

    public string GetTextValue()
    {
        return (Mathf.Round(value / step) * step).ToString(format);
    }
}
