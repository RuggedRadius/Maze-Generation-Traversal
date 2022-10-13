using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RotateDirection
{
    Left,
    Right
}

public class Rotation : MonoBehaviour
{
    public void Rotate(RotateDirection dir)
    {
        if (dir == RotateDirection.Left)
        {
            this.transform.eulerAngles += Vector3.up * -90f;
        }
        else
        {
            this.transform.eulerAngles += Vector3.up * 90f;
        }
    }
}
