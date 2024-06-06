using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopupManager : MonoBehaviour
{
    public List<Popup> popups { get; } = new List<Popup>();

    public static PopupManager instance { get; private set; }

    private void Awake()
    {
        Init();
        instance = this;
    }

    public void Init()
    {
        popups.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            var obj = transform.GetChild(i);
            var popup = obj.GetComponent<Popup>();
            if (popup != null)
            {
                popup.Init();
                popups.Add(popup);
            }
        }
    }

    public Popup GetPopup(string key)
    {
        return popups.Find(e => e.key == key);
    }

    public Popup ShowPopup(string key, PopupOptions options) => ShowPopup(GetPopup(key), options);
    public Popup ShowPopup(Popup prefab, PopupOptions options)
    {
        var popup = ObjectPool.global.Spawn(prefab, transform);
        popup.transform.SetAsLastSibling();
        popup._Setup(options);
        return popup;
    }

    public Popup ShowAlert(string text) => ShowPopup("alert", new PopupOptions()
    {
        data = new AlertPopupData() { text = text }
    });
    public Popup ShowConfirmation(string text, UnityAction action) => ShowPopup("confirmation", new PopupOptions()
    {
        data = new ConfirmationPopupData() { text = text, onConfirm = action }
    });
}
