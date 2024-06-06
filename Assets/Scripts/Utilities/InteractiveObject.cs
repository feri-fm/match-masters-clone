using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveObject : MonoBehaviour
{
    public bool isInteractive { get; set; } = true;

    public bool isDown { get; private set; } = false;
    public bool isDragging { get; private set; } = false;

    private float _downTime;
    private Vector3 _downPosition;

    public const float CLICK_TIME = 0.3f;
    public const float CLICK_DELTA = 10;
    public const float DRAG_DELTA = 10;

    public event UnityAction onClick = () => { };
    public event UnityAction onDown = () => { };
    public event UnityAction onUp = () => { };
    public event UnityAction onDragStart = () => { };
    public event UnityAction onDragEnd = () => { };
    public event UnityAction onDrag = () => { };

    public bool allowed => isInteractive && !UIFilter.IsPointerClear();

    public float dragDistance => (Input.mousePosition - _downPosition).magnitude;

    public virtual void OnMouseDown()
    {
        if (allowed)
        {
            _downTime = Time.time;
            _downPosition = Input.mousePosition;
            isDown = true;
            isDragging = false;
            onDown.Invoke();
        }
    }
    public virtual void OnMouseDrag()
    {
        if (!isDragging && isDown && dragDistance > DRAG_DELTA)
        {
            isDragging = true;
            OnDragStart();
            onDragStart.Invoke();
        }
        if (isDragging)
        {
            OnDrag();
            onDrag.Invoke();
        }
    }
    public virtual void OnMouseUp()
    {
        if (isDown && isDragging)
        {
            isDragging = false;
            OnDragEnd();
            onDragEnd.Invoke();
        }
        if (isDown)
        {
            onUp.Invoke();
        }
        isDown = false;
    }
    public virtual void OnMouseUpAsButton()
    {
        if (allowed && isDown && Time.time < _downTime + CLICK_TIME
            && dragDistance < CLICK_DELTA)
        {
            OnClick();
            onClick.Invoke();
        }
    }

    public virtual void OnClick() { }
    public virtual void OnDragStart() { }
    public virtual void OnDrag() { }
    public virtual void OnDragEnd() { }

    public Vector3 GetMousePosition()
    {
        var dist = transform.position.z - Camera.main.transform.position.z;
        var point = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * dist);
        return point;
    }
}
