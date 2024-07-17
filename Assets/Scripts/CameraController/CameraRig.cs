using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    public float size = 5;
    public Vector2 anchor = new Vector2(0.5f, 0.5f);
    public bool fitWidth;

    public Rect GetRect()
    {
        var ratio = (float)Screen.width / Screen.height;
        var s = new Vector2(size * ratio, size);
        if (fitWidth)
            s = new Vector2(size, size / ratio);
        var pos = transform.position - new Vector3(anchor.x * s.x, anchor.y * s.y);
        var rect = new Rect(pos, s);
        return rect;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        DrawGizmos();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        DrawGizmos();
    }
    private void DrawGizmos()
    {
        var rect = GetRect();
        Gizmos.DrawWireCube(rect.center, rect.size);
    }
}
