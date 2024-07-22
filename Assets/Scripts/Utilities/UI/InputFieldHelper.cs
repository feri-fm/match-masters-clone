using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_InputField))]
public class InputFieldHelper : MonoBehaviour
{
    public Button.ButtonClickedEvent onStartEdit;

    public TMP_InputField inputField => _inputField ?? Load(); TMP_InputField _inputField;

    private string _text;
    private bool _isEditing = false;

    private void Awake()
    {
        Load();
    }

    public string text
    {
        get => _text;
        set => SetText(value);
    }

    public void SetTextWithoutNotify(string text)
    {
        _text = text;
        inputField.SetTextWithoutNotify(text);
    }

    public void SetText(string text)
    {
        _text = text;
        inputField.text = text;
    }

    public TMP_InputField Load()
    {
        _isEditing = false;
        _inputField = GetComponent<TMP_InputField>();
        SetTextWithoutNotify(_inputField.text);
        _inputField.onValueChanged.AddListener((value) =>
        {
            if (!_isEditing)
            {
                onStartEdit?.Invoke();
                _isEditing = true;
            }
            SetTextWithoutNotify(value);
        });
        _inputField.onEndEdit.AddListener((value) =>
        {
            _isEditing = false;
        });
        return _inputField;
    }
}
