using System.Collections;
using System.Collections.Generic;
using MMC.Game;
using UnityEngine;

public class PanelCamera : MonoBehaviour
{
    public Panel panel;
    public CameraRig cameraRig;

    private void Awake()
    {
        panel.onOpen += () =>
        {
            if (cameraRig != null)
                GameManager.instance.cameraController.SetRig(cameraRig);
        };
    }

    private void Reset()
    {
        panel = GetComponent<Panel>();
    }
}
