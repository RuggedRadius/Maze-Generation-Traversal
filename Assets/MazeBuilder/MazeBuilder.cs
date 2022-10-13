using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeBuilder : MonoBehaviour
{
    public int cellCount;
    public int wallCount;

    public float wallHeight;
    public float wallThickness;

    public float baseOverhang;
    public float baseThickness;

    public Color baseColour;
    public Color wallColour;

    public float wallBuildTime;

    public GameObject prefabCellDetector;

    public Material matUnvisited;
    public Material matVisited;
    public Material matCurrentPos;
    public Material matDecisionPoint;

    private GameObject mazeParent;
    private MazeGenerator mazeGen;
    private CellLinker cellLinker;

    private void Awake()
    {
        mazeGen = this.GetComponent<MazeGenerator>();
        cellLinker = this.GetComponent<CellLinker>();
    }

    public void BuildMaze()
    {
        // Create parent object
        mazeGen = this.GetComponent<MazeGenerator>();
        mazeParent = new GameObject($"Maze_{mazeGen.mazeSize.x}x{mazeGen.mazeSize.y}");

        // Generate Cells
        for (int i = 0; i < mazeGen.mazeSize.x; i++)
        {
            for (int j = 0; j < mazeGen.mazeSize.y; j++)
            {
                MazeCell cell = mazeGen.mazeData[i, j];

                // Create cell
                int xPos = cell.xPosition;
                int yPos = cell.yPosition;
                GameObject cellObject = new GameObject($"Cell_[{xPos}, {yPos}]");
                cellObject.transform.SetParent(mazeParent.transform);
                cell.cellObject = cellObject;

                // Position cell
                cellObject.transform.localPosition = new Vector3(xPos, 0, yPos);

                // Build Walls
                for (int w = 0; w < cell.walls.Length; w++)
                {
                    if (cell.walls[w] != null)
                    {
                        BuildWall(cellObject, cell, cell.walls[w]);
                    }

                    //yield return null;
                }
            }
        }

        // Build base
        BuildBase(mazeGen.mazeSize);

        // Create detectors
        CreateCellDetectors();

        Debug.Log("Maze building complete");
    }

    private bool isNeighbouringWallAlreadyExist(MazeCell cell, MazeWall wall)
    {
        MazeCell neighbourCell = null;

        switch (wall.face)
        {
            case WallFace.North:
                neighbourCell = cell.neighbourNorth;
                if (neighbourCell != null && neighbourCell.walls[2].wallObject != null)
                {
                    return true;
                }
                break;

            case WallFace.East:
                neighbourCell = cell.neighbourEast;
                if (neighbourCell != null && neighbourCell.walls[3].wallObject != null)
                {
                    return true;
                }
                break;

            case WallFace.South:
                neighbourCell = cell.neighbourSouth;
                if (neighbourCell != null && neighbourCell.walls[0].wallObject != null)
                {
                    return true;
                }
                break;

            case WallFace.West:
                neighbourCell = cell.neighbourWest;
                if (neighbourCell != null && neighbourCell.walls[1].wallObject != null)
                {
                    return true;
                }
                break;
        }

        return false;
    }

    private void BuildWall(GameObject cellObject, MazeCell cell, MazeWall wall)
    {
        if (isNeighbouringWallAlreadyExist(cell, wall))
            return;

        // Create geometry
        GameObject wallObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallObject.transform.SetParent(cellObject.transform);

        // Set colour
        wallObject.GetComponent<Renderer>().material.color = cell.cellColour;
        //SetWallColour(wallObject, wall.face);

        // Increment wall count
        wallCount++;

        // Change name
        wallObject.name = $"Wall_{wall.face.ToString()}_{wallCount}";

        // Size wall
        Vector3 scale = new Vector3(1.1f, wallHeight, wallThickness);
        wallObject.transform.localScale = scale;
        wallObject.GetComponent<BoxCollider>().size = scale;

        // Rotate wall
        RotateWall(wallObject, wall.face);

        // Position wall
        PositionWallInCell(wallObject, wall.face);

        wallObject.tag = "Wall";

        wall.wallObject = wallObject;

        wall.hasBeenBuilt = true;
    }

    private void RotateWall(GameObject wall, WallFace direction)
    {
        Vector3 rotationToApply = Vector3.zero;

        switch (direction)
        {
            case WallFace.North:
                wall.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                break;

            case WallFace.East:
                wall.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
                break;

            case WallFace.South:
                wall.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                break;

            case WallFace.West:
                wall.transform.localEulerAngles = new Vector3(0f, 270f, 0f);
                break;
        }
    }

    private void BuildBase(Vector2 size)
    {
        GameObject baseFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        baseFloor.name = "Base";
        baseFloor.transform.SetParent(mazeParent.transform);

        baseFloor.transform.localScale = new Vector3(size.x + baseOverhang, baseThickness, size.y + baseOverhang);

        float xPos = (((float)size.x) / 2) - 0.5f;
        float yPos = -wallHeight;
        float zPos = (((float)size.y) / 2) - 0.5f;

        baseFloor.transform.localPosition = new Vector3(xPos, yPos, zPos);

        baseFloor.GetComponent<Renderer>().material.color = baseColour;
    }

    private void PositionWallInCell(GameObject wall, WallFace direction)
    {
        Vector3 displacement = Vector3.zero;

        float cellSize = 0.5f;

        switch (direction)
        {
            case WallFace.North:
                displacement = new Vector3(0f, 0f, cellSize);
                break;

            case WallFace.East:
                displacement = new Vector3(cellSize, 0f, 0f);
                break;

            case WallFace.South:
                displacement = new Vector3(0f, 0f, -cellSize);
                break;

            case WallFace.West:
                displacement = new Vector3(-cellSize, 0f, 0f);
                break;
        }

        wall.transform.localPosition = displacement;
    }

    private void CreateCellDetectors()
    {
        foreach (MazeCell cell in mazeGen.mazeData)
        {
            // Create collider
            GameObject newDetector = Instantiate(prefabCellDetector);
            cell.cellDetectorObject = newDetector;

            // Set parent
            newDetector.transform.SetParent(cell.cellObject.transform);

            // Position detector
            newDetector.transform.position = new Vector3(cell.xPosition, 0f, cell.yPosition);

            newDetector.GetComponent<MeshRenderer>().material = matUnvisited;


        }
    }
}
