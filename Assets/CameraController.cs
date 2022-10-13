using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool followNavigator;
    [Range(1f, 500f)] public float cameraHeight;
    [Range(1f, 10f)] public float zoomSensitivity;

    private GameObject target;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (followNavigator)
            this.transform.position = CalculateCameraPosition();

        this.transform.position -= Vector3.up * Input.mouseScrollDelta.y * zoomSensitivity;

        if (Input.GetKey(KeyCode.W))
        {
            this.transform.position += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.position += Vector3.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.transform.position += Vector3.back;
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.position += Vector3.right;
        }
    }

    private Vector3 CalculateCameraPosition()
    {
        float x = target.transform.position.x;
        float y = target.transform.position.y + cameraHeight;
        float z = target.transform.position.z;

        y = Mathf.Clamp(y, 1, 100);

        return new Vector3(x, y, z);
    }
}
