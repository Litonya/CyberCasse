using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AeliTempCamera : MonoBehaviour
{
    public void RotateLeft()
    {
        transform.Rotate(Vector3.up, 90f, Space.Self);
    }

    public void RotateRight()
    {
        transform.Rotate(Vector3.up, -90f, Space.Self);
    }
}
