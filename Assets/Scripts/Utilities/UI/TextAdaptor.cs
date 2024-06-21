using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public abstract class TextAdaptor : MonoBehaviour
{
    public abstract string text { get; set; }

    public UnityAction onChanged = () => { };

    public abstract void SetText(string text);

    public abstract void SetTextWithoutNotify(string text);
}
