using Assets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public Vector2Int mazeSize;

    public MazeCell[,] mazeData;

    private MazeBuilder builder;
    private CellLinker cellLinker;
    private Navigate nav;

    public bool mazeGenerated;

    void Awake()
    {
        builder = GetComponent<MazeBuilder>();
        cellLinker = GetComponent<CellLinker>();
        nav = GameObject.FindGameObjectWithTag("Player").GetComponent<Navigate>();
    }

    public void GenerateMaze(Vector2Int size)
    {
        mazeSize = size;
        StartCoroutine(GenMazeComplete());
    }

    private IEnumerator GenMazeComplete()
    {
        mazeGenerated = false;

        if (nav.visitedCells != null)
            nav.visitedCells.Clear();
        nav.visitedCells = new List<MazeCell>();
        builder.cellCount = 0;

        // Delete existing maze, if any        
        while (nav.move.isMoving || nav.isTravellingDuringRetrace)
        {
            yield return null;
        }
        nav.state = NavigationState.Idle;
        Destroy(builder.mazeParent);


        Debug.Log("Generating maze grid...");
        GenerateMazeGrid(mazeSize);

        Debug.Log("Building maze grid...");
        builder.BuildMaze();

        Debug.Log("Linking maze cells ...");
        LinkCells();

        Debug.Log("Generating openings of maze...");
        SetMazeEntrance(false);
        SetMazeExit(false);

        mazeGenerated = true;

        yield return null;

        // Start navigation
        nav.Initialise();
    }

    public void SetMazeEntrance(bool active)
    {
        MazeCell start = mazeData[0, 0];
        MazeWall startWall = start.walls[3];

        startWall.wallObject.SetActive(active);
    }

    public void SetMazeExit(bool active)
    {
        MazeCell end = mazeData[mazeSize.x - 1, mazeSize.y - 1];
        MazeWall endWall = end.walls[1];

        if (endWall == null)
            endWall = end.walls[0];

        endWall.wallObject.SetActive(active);
    }

    public void LinkCells()
    {

        // Get random cell
        //MazeCell initialCell = mazeGen.GetRandomCell();
        MazeCell initialCell = mazeData[0, 0];

        // Mark as part of the maze
        initialCell.hasVisited = true;

        // Create walls list
        List<MazeWall> wallList = new List<MazeWall>();

        // Add walls of cell to wall list
        for (int i = 0; i < initialCell.walls.Length; i++)
        {
            if (initialCell.walls[i] != null)
            {
                wallList.Add(initialCell.walls[i]);
            }
        }

        // While walls remain in list
        while (wallList.Count > 0)
        {
            // Pick random wall from wall list
            MazeWall randomWall = wallList[Random.Range(0, wallList.Count)];

            if (randomWall.isEdge == false)
            {
                // Get cell visited states
                bool cell1Visited = randomWall.cell1.hasVisited;
                bool cell2Visited = false;
                if (randomWall.cell2 != null)
                {
                    cell2Visited = randomWall.cell2.hasVisited;
                }

                // Mark un-visited cell as visited
                if (!cell1Visited && cell2Visited)
                {
                    randomWall.cell1.hasVisited = true;
                    randomWall.cell1.DeleteWall(randomWall);
                    DestroyWall(randomWall);

                    for (int i = 0; i < randomWall.cell1.walls.Length; i++)
                    {
                        if (randomWall.cell1.walls[i] != null)
                            wallList.Add(randomWall.cell1.walls[i]);
                    }
                }

                if (cell1Visited && !cell2Visited)
                {
                    if (randomWall.cell2 != null)
                    {
                        randomWall.cell2.hasVisited = true;
                        randomWall.cell2.DeleteWall(randomWall);
                        DestroyWall(randomWall);

                        for (int i = 0; i < randomWall.cell2.walls.Length; i++)
                        {
                            if (randomWall.cell2.walls[i] != null)
                                wallList.Add(randomWall.cell2.walls[i]);
                        }
                    }
                }
            }

            // Remove wall from list
            wallList.Remove(randomWall);
        }
    }

    public void DestroyWall(MazeWall wall)
    {
        if (wall.isEdge)
            return;

        DestroyImmediate(wall.wallObject);

        try
        {
            switch (wall.face)
            {
                case WallFace.North:
                    DestroyImmediate(wall.cell2.walls[2].wallObject);
                    break;

                case WallFace.East:
                    DestroyImmediate(wall.cell2.walls[3].wallObject);
                    break;

                case WallFace.South:
                    DestroyImmediate(wall.cell2.walls[0].wallObject);
                    break;

                case WallFace.West:
                    DestroyImmediate(wall.cell2.walls[1].wallObject);
                    break;
            }
        }
        catch (System.Exception) { }
    }

    private void GenerateMazeGrid(Vector2 size)
    {
        mazeData = new MazeCell[mazeSize.x, mazeSize.y];

        // Create cells
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                mazeData[x, y] = new MazeCell();
                mazeData[x, y].cellColour = builder.wallColour;
                mazeData[x, y].mazeGen = this;
                mazeData[x, y].xPosition = x;
                mazeData[x, y].yPosition = y;
            }
        }

        // Assign neighbours
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                try
                {
                    mazeData[x, y].neighbourNorth = mazeData[x, y + 1];
                }
                catch (System.Exception) { }

                try
                {
                    mazeData[x, y].neighbourEast = mazeData[x + 1, y];
                }
                catch (System.Exception) { }

                try
                {
                    mazeData[x, y].neighbourSouth = mazeData[x, y - 1];
                }
                catch (System.Exception) { }

                try
                {
                    mazeData[x, y].neighbourWest = mazeData[x - 1, y];
                }
                catch (System.Exception) { }
            }
        }

        // Add walls
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                MazeCell cell = mazeData[x, y];

                // Add all walls to cell initially.
                AddWallToCell(cell, WallFace.North);
                AddWallToCell(cell, WallFace.East);
                AddWallToCell(cell, WallFace.South);
                AddWallToCell(cell, WallFace.West);
            }
        }
    }
    public MazeCell GetRandomCell()
    {
        int xCoord = Random.Range(0, mazeSize.x - 1);
        int yCoord = Random.Range(0, mazeSize.y - 1);

        return mazeData[xCoord, yCoord];
    }

    private void AddWallToCell(MazeCell cell, WallFace wall)
    {
        // Create new wall
        MazeWall newWall = new MazeWall();

        // Assign face
        newWall.face = wall;

        // Determine neighbour cell
        MazeCell neighbourCell = null;
        switch (wall)
        {
            case WallFace.North: neighbourCell = cell.neighbourNorth; break;
            case WallFace.East: neighbourCell = cell.neighbourEast; break;
            case WallFace.South: neighbourCell = cell.neighbourSouth; break;
            case WallFace.West: neighbourCell = cell.neighbourWest; break;
        }

        // Assign cells
        newWall.cell1 = cell;
        newWall.cell2 = neighbourCell;

        // Make edge walls permanent
        switch (wall)
        {
            case WallFace.North:
                if (cell.yPosition == mazeSize.y - 1)
                {
                    newWall.isEdge = true;
                }
                break;

            case WallFace.East:
                if (cell.xPosition == mazeSize.x - 1)
                {
                    newWall.isEdge = true;
                }
                break;

            case WallFace.South:
                if (cell.xPosition == 0)
                {
                    newWall.isEdge = true;
                }
                break;

            case WallFace.West:
                if (cell.xPosition == 0)
                {
                    newWall.isEdge = true;
                }
                break;
        }

        // Add walls to cells
        cell.AddWall(newWall);
    }
}
