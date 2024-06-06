using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationPopup : Popup<ConfirmationPopupData>
{
    public TextHelper text;
    public TextHelper confirm;
    public TextHelper cancel;
    public ButtonHelper btnConfirm;
    public ButtonHelper btnCancel;

    private string defConfirm;
    private string defCancel;

    public override void OnCreated()
    {
        base.OnCreated();
        defConfirm = confirm.text;
        defCancel = cancel.text;
    }

    public override void Setup()
    {
        base.Setup();
        text.text = data.text;
        confirm.text = data.confirm == "" ? defConfirm : data.confirm;
        cancel.text = data.cancel == "" ? defCancel : data.cancel;

        btnConfirm.onClick.RemoveAllListeners();
        btnCancel.onClick.RemoveAllListeners();
        btnConfirm.onClick.AddListener(data.onConfirm);
        btnConfirm.onClick.AddListener(Close);
        btnCancel.onClick.AddListener(data.onCancel);
        btnCancel.onClick.AddListener(Close);
    }
}
public class ConfirmationPopupData
{
    public string text;
    public string confirm = "";
    public string cancel = "";
    public UnityAction onConfirm;
    public UnityAction onCancel;
}
