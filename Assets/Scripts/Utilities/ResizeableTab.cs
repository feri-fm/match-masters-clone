using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResizeableTab : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public Canvas canvas;
    public RectTransform rect;
    public Mode mode;
    public float smooth = 0.4f;
    public float speed = 100;
    public float value = 0;
    public float[] grids = new float[] { 40, 400 };

    private bool isDragging;
    private Vector2 startValue;
    private float onDown = 0;
    private Vector2 startPos;
    private float clickTime = 0.3f;
    private float clickRadius = 20;

    public enum Mode
    {
        Horizontal, Vertical
    }

    private void FixedUpdate()
    {
        if (!isDragging)
        {
            switch (mode)
            {
                case Mode.Horizontal:
                    rect.sizeDelta = Vector2.Lerp(rect.sizeDelta, new Vector2(value, rect.sizeDelta.y), smooth);
                    break;
                case Mode.Vertical:
                    rect.sizeDelta = Vector2.Lerp(rect.sizeDelta, new Vector2(rect.sizeDelta.x, value), smooth);
                    break;
            }
        }
    }

    public virtual void OnDrag(PointerEventData e)
    {
        isDragging = true;
        Vector2 delta = startValue - e.position * speed / canvas.transform.localScale.x;
        switch (mode)
        {
            case Mode.Horizontal:
                value = delta.x;
                rect.sizeDelta = new Vector2(value, rect.sizeDelta.y);
                break;
            case Mode.Vertical:
                value = delta.y;
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, value);
                break;
        }
    }

    public virtual void OnPointerDown(PointerEventData e)
    {
        startValue = e.position * speed / canvas.transform.localScale.x + Vector2.one * value;
        startPos = e.position;
        onDown = Time.time;
    }

    public virtual void OnPointerUp(PointerEventData e)
    {
        isDragging = false;
        Snap();
        if (Time.time < onDown + clickTime && (startPos - e.position).magnitude < clickRadius)
        {
            if (value == grids[0])
                value = grids[1];
            else
                value = grids[0];
        }
    }

    public void Snap()
    {
        for (int i = 0; i < grids.Length; i++)
        {
            if (value < grids[0])
            {
                value = grids[0];
                return;
            }
            else if (i < grids.Length - 1 && value > grids[i] && value < grids[i + 1])
            {
                if (value - grids[i] < grids[i + 1] - value)
                    value = grids[i];
                else
                    value = grids[i + 1];
                return;
            }
            else if (i == grids.Length - 1 && value > grids[i])
            {
                value = grids[i];
                return;
            }
        }
    }
}
