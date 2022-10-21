using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FacingDirection
{
    North,
    East,
    South,
    West
}

public class Movement : MonoBehaviour
{
    public bool isMoving;
    public bool isRotating;

    [Range(0.01f, 1f)] public float movementDuration;

    private Navigate nav;
    private Rotation rotate;

    private void Awake()
    {
        nav = this.GetComponent<Navigate>();
        rotate = this.GetComponent<Rotation>();
    }

    public IEnumerator MoveToCell(MazeCell cell)
    {
        if (isMoving)
            yield break;

        if (nav.finishedMaze)
            yield break;

        isMoving = true;

        yield return null;

        float timer = 0f;

        Vector3 startPosition = this.transform.position;
        Vector3 endPosition = this.transform.position;
        try
        {
            endPosition = cell.cellObject.transform.position;
        }
        catch (System.Exception)
        {
            yield break;
        }

        while (this.transform.position != endPosition)
        {
            timer += Time.deltaTime;

            this.transform.position = Vector3.Lerp(startPosition, endPosition, timer / movementDuration);

            yield return null;
        }

        nav.lastPosition = endPosition;

        isMoving = false;
    }

    public void RotateNavigator(MazeCell currentCell, MazeCell nextCell)
    {
        StartCoroutine(Rotate(currentCell, nextCell));
    }

    private IEnumerator Rotate(MazeCell currentCell, MazeCell nextCell)
    {
        isRotating = true;

        int deltaX = currentCell.xPosition - nextCell.xPosition;
        int deltaY = currentCell.yPosition - nextCell.yPosition;

        Vector3 startRotation = this.transform.eulerAngles;
        Vector3 endRotation = Vector3.zero;

        if (deltaX == 0 && deltaY == 1)
        {
            endRotation = new Vector3(0f, 180f, 0f);
        }
        else if (deltaX == 1 && deltaY == 0)
        {
            endRotation = new Vector3(0f, -90f, 0f);
        }
        else if (deltaX == -1 && deltaY == 0)
        {
            endRotation = new Vector3(0f, 90f, 0f);
        }
        else if (deltaX == 0 && deltaY == -1)
        {
            endRotation = new Vector3(0f, 0f, 0f);
        }

        float timer = 0f;
        while (transform.eulerAngles != endRotation)
        {
            timer += Time.deltaTime;

            this.transform.eulerAngles = Vector3.Lerp(startRotation, endRotation, timer / movementDuration);

            yield return null;
        }

        isRotating = false;

        yield return null;
    }

    public IEnumerator RotateInDirection(LookDirection direction)
    {
        isRotating = true;

        Vector3 startRotation = this.transform.eulerAngles;
        Vector3 endRotation = Vector3.zero;

        switch (direction)
        {
            case LookDirection.Left:
                endRotation = startRotation + new Vector3(0f, -90f, 0f);
                break;

            case LookDirection.Right:
                endRotation = startRotation + new Vector3(0f, 90f, 0f);
                break;

            case LookDirection.Backward:
                endRotation = startRotation + new Vector3(0f, -180f, 0f);
                break;

            case LookDirection.Forward:
                break;
        }

        float timer = 0f;
        while (transform.eulerAngles != endRotation)
        {
            timer += Time.deltaTime;

            this.transform.eulerAngles = Vector3.Lerp(startRotation, endRotation, timer / movementDuration);

            yield return null;
        }

        isRotating = false;

        yield return null;
    }    
}
