using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSpinner : MonoBehaviour
{
    public Transform anchor;
    public float speed = 360;

    private void Update()
    {
        anchor.Rotate(Vector3.forward * speed * Time.deltaTime);
    }
}
