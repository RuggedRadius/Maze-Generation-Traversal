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
}
