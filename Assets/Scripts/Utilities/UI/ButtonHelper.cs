using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHelper : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent onClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        var s = GetComponent<Selectable>();
        if (s == null || s.interactable)
            onClick.Invoke();
    }
}
