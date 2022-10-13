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

    public float movementDuration;

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

        isMoving = true;

        float timer = 0f;

        Vector3 startPosition = this.transform.position;
        Vector3 endPosition = cell.cellObject.transform.position;

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
        int deltaX = currentCell.xPosition - nextCell.xPosition;
        int deltaY = currentCell.yPosition - nextCell.yPosition;

        if (deltaX == 0 && deltaY == 1)
            this.transform.eulerAngles = new Vector3(0f, 180f, 0f);
        else if (deltaX == 1 && deltaY == 0)
            this.transform.eulerAngles = new Vector3(0f, -90f, 0f);
        else if (deltaX == -1 && deltaY == 0)
            this.transform.eulerAngles = new Vector3(0f, -270f, 0f);
        else if (deltaX == 0 && deltaY == -1)
            this.transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }

    //public IEnumerator Move(LookDirection moveDir)
    //{
    //    switch (moveDir)
    //    {
    //        case LookDirection.Left:
    //            rotate.Rotate(RotateDirection.Left);
    //            StartCoroutine(MoveForward());
    //            break;

    //        case LookDirection.Right:
    //            rotate.Rotate(RotateDirection.Right);
    //            StartCoroutine(MoveForward());
    //            break;

    //        case LookDirection.Forward:
    //            StartCoroutine(MoveForward());
    //            break;

    //        case LookDirection.Backward:
    //            rotate.Rotate(RotateDirection.Right);
    //            rotate.Rotate(RotateDirection.Right);
    //            StartCoroutine(MoveForward());
    //            break;
    //    }

    //    yield return null;
    //}

    //public IEnumerator MoveForward()
    //{
    //    if (isMoving)
    //        yield break;

    //    isMoving = true;

    //    float timer = 0f;

    //    Vector3 startPosition = this.transform.position;
    //    Vector3 endPosition = startPosition + transform.forward;

    //    while (this.transform.position != endPosition)
    //    {
    //        timer += Time.deltaTime;

    //        this.transform.position = Vector3.Lerp(startPosition, endPosition, timer / movementDuration);

    //        yield return null;
    //    }

    //    nav.lastPosition = endPosition;

    //    isMoving = false;
    //}

    //public FacingDirection CalculateFacingDirection()
    //{
    //    if (transform.eulerAngles.y == 0 || transform.eulerAngles.y == 360)
    //        return FacingDirection.North;
    //    else if (transform.eulerAngles.y == 90 || transform.eulerAngles.y == -270)
    //        return FacingDirection.East;
    //    else if (transform.eulerAngles.y == 180 || transform.eulerAngles.y == -180)
    //        return FacingDirection.South;
    //    else if (transform.eulerAngles.y == -90 || transform.eulerAngles.y == 270)
    //        return FacingDirection.West;
    //    else
    //    {
    //        Debug.Log($"ERROR calculating facing direction [{transform.eulerAngles.y}]");
    //        return FacingDirection.North;
    //    }
    //}

    //public Vector2Int CalculateNewCellPosition(FacingDirection facingDirection)
    //{
    //    Vector2Int positionDelta = Vector2Int.zero;

    //    switch (facingDirection)
    //    {
    //        case FacingDirection.North:
    //            positionDelta = new Vector2Int(0, 1);
    //            break;

    //        case FacingDirection.East:
    //            positionDelta = new Vector2Int(1, 0);
    //            break;

    //        case FacingDirection.South:
    //            positionDelta = new Vector2Int(0, -1);
    //            break;

    //        case FacingDirection.West:
    //            positionDelta = new Vector2Int(-1, 0);
    //            break;
    //    }

    //    return nav.currentCellPos + positionDelta;
    //}

    //public IEnumerator MoveTo(Vector3 endPosition, float duration)
    //{
    //    if (isMoving)
    //        yield break;

    //    duration = duration / movementSpeed;

    //    isMoving = true;

    //    float timer = 0f;

    //    Vector3 startPosition = this.transform.position;

    //    while (this.transform.position != endPosition)
    //    {
    //        timer += Time.deltaTime;

    //        this.transform.position = Vector3.Lerp(startPosition, endPosition, timer / duration);

    //        yield return null;
    //    }

    //    nav.lastPosition = endPosition;

    //    isMoving = false;
    //}
}
