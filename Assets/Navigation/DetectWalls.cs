using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LookDirection
{
    Left,
    Forward,
    Right,
    Backward
}

public class DetectWalls : MonoBehaviour
{
    public float fieldOfVisionDistance;

    public bool DetectWallInAbsoluteDirection(FacingDirection direction)
    {
        Ray ray = new Ray(this.transform.position, Vector3.zero);

        switch (direction)
        {
            case FacingDirection.North:
                ray.direction = Vector3.forward;
                break;

            case FacingDirection.East:
                ray.direction = Vector3.right;
                break;

            case FacingDirection.South:
                ray.direction = Vector3.back;
                break;

            case FacingDirection.West:
                ray.direction = Vector3.left;
                break;
        }

        RaycastHit hit;
        Physics.Raycast(ray.origin, ray.direction, out hit, fieldOfVisionDistance);


        if (hit.collider != null)
        {
            //Debug.Log($"Hit: {hit.collider.name}");
            Debug.DrawRay(ray.origin, ray.direction * fieldOfVisionDistance, Color.red, 3f);

            if (hit.collider.gameObject.tag == "Wall")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * fieldOfVisionDistance, Color.green, 3f);
            return false;
        }
    }

    public bool DetectWallInDirection(LookDirection direction)
    {
        Ray ray = new Ray(this.transform.position, Vector3.zero);        

        switch (direction)
        {
            case LookDirection.Left:
                ray.direction = -transform.right; 
                break;

            case LookDirection.Forward:
                ray.direction = transform.forward;
                break;

            case LookDirection.Right:
                ray.direction = transform.right;
                break;
        }

        RaycastHit hit;
        Physics.Raycast(ray.origin, ray.direction, out hit, fieldOfVisionDistance);
        

        if (hit.collider != null)
        {
            //Debug.Log($"Hit: {hit.collider.name}");
            Debug.DrawRay(ray.origin, ray.direction * fieldOfVisionDistance, Color.red, 3f);

            if (hit.collider.gameObject.tag == "Wall")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * fieldOfVisionDistance, Color.green, 3f);
            return false;
        }
    }
    
}
