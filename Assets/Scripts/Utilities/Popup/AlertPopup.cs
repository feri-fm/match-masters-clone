using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertPopup : Popup<AlertPopupData>
{
    public TextAdaptor text;

    public override void Setup()
    {
        base.Setup();
        text.text = data.text;
    }
}
public class AlertPopupData
{
    public string text;
}
