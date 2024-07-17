using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera cam;

    public CameraRig rig;

    public void SetRig(CameraRig rig)
    {
        this.rig = rig;
    }

    private void Update()
    {
        if (rig != null)
        {
            var rect = rig.GetRect();
            transform.rotation = rig.transform.rotation;
            transform.position = rect.center;
            cam.orthographicSize = rect.height / 2;
        }
    }
}
