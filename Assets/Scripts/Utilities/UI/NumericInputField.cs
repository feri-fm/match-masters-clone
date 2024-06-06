using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumericInputField : MonoBehaviour
{
    public InputFieldHelper inputField;

    public float _value;

    public bool integer = true;

    public Button.ButtonClickedEvent onStartEdit;
    public Button.ButtonClickedEvent onValueChanged;
    public Button.ButtonClickedEvent onEndEdit;

    private bool _isEditing = false;

    private void Awake()
    {
        inputField.inputField.onValueChanged.AddListener((value) =>
        {
            if (!_isEditing)
            {
                onStartEdit?.Invoke();
                _isEditing = true;
            }
            if (integer)
            {
                if (int.TryParse(value, out int i)) SetValue(i);
            }
            else
            {
                if (float.TryParse(value, out float f)) SetValue(f);
            }
        });
        inputField.inputField.onEndEdit.AddListener((value) =>
        {
            onEndEdit?.Invoke();
            inputField.text = _value.ToString();
            _isEditing = false;
        });
    }

    public float value
    {
        get => _value;
        set => SetValue(value);
    }

    public void SetValue(float value)
    {
        _value = value;
        inputField.SetTextWithoutNotify(value.ToString());
        onValueChanged.Invoke();
    }
    public void SetValueWithoutNotify(float value)
    {
        _value = value;
        inputField.SetTextWithoutNotify(value.ToString());
    }
    public float GetValue()
    {
        return _value;
    }
}
