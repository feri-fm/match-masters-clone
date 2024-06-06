using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TextHelper : MonoBehaviour
{
    public bool ignoreCustomText;

    [TextArea(1, 10)]
    public string _text;

    private TMP_Text _tmpText;

    public TMP_Text tmpText => _tmpText ?? Load();

    private string _displayText;

    public string text { get => _text ?? tmpText.text; set => SetText(value); }

    public UnityAction onChanged = () => { };

    public TMP_Text Load()
    {
        if (_tmpText == null)
        {
            _tmpText = GetComponent<TMP_Text>();
            _displayText = _text;
        }
        return _tmpText;
    }

    public void SetText(string text)
    {
        Load();
        _text = text;
        _displayText = text;
        onChanged.Invoke();
        UpdateText();
    }

    public void SetTextWithoutNotify(string text)
    {
        Load();
        _text = text;
        _displayText = text;
        UpdateText();
    }

    public void SetDisplayText(string text)
    {
        Load();
        _displayText = text;
        UpdateText();
    }

    public void UpdateText() => UpdateText(_displayText);
    public void UpdateText(string text)
    {
        tmpText.text = text;
    }

    private void OnValidate()
    {
        if (!ignoreCustomText)
        {
            if (_text == "")
                _text = tmpText.text;
            SetText(_text);
        }
    }
}
