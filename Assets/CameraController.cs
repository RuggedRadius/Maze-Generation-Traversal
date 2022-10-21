using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool followNavigator;
    public bool pointOfViewEnabled;

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
        {
            Vector3 camPos = CalculateFollowPosition();
            camPos.y -= Input.mouseScrollDelta.y * zoomSensitivity;
            this.transform.position = camPos;

            this.transform.eulerAngles = new Vector3(84f, 0f, 0f);
        }
        else if (pointOfViewEnabled)
        {
            this.transform.position = CalculatePOVPosition();

            this.transform.eulerAngles = target.transform.eulerAngles;
        }
        else
        {
            this.transform.position -= Vector3.up * Input.mouseScrollDelta.y * zoomSensitivity;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                this.transform.position += transform.up * 0.1f;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                this.transform.position += -transform.right * 0.1f;
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                this.transform.position += -transform.up * 0.1f;
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                this.transform.position += transform.right * 0.1f;
            }
            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.PageDown))
            {
                this.transform.eulerAngles += transform.up * 1f;
            }
            if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.PageUp))
            {
                this.transform.eulerAngles += -transform.up * 1f;
            }

            transform.eulerAngles = new Vector3(
                Mathf.Clamp(transform.eulerAngles.x, 84f, 84f),
                transform.eulerAngles.y,
                Mathf.Clamp(transform.eulerAngles.z, 0f, 0f)
                );
        }
    }

    private Vector3 CalculateFollowPosition()
    {
        float x = target.transform.position.x;
        float y = this.transform.position.y;
        float z = target.transform.position.z;

        y = Mathf.Clamp(y, 1, 100);

        return new Vector3(x, y, z);
    }

    private Vector3 CalculatePOVPosition()
    {
        float x = target.transform.position.x;
        float y = target.transform.position.y;
        float z = target.transform.position.z;

        return new Vector3(x, y, z);
    }
}
