using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellLinker : MonoBehaviour
{
    public MazeGenerator mazeGen;

    public MazeBuilder builder;

    public bool linkingComplete;



    private void Awake()
    {
        mazeGen = this.GetComponent<MazeGenerator>();
        builder = this.GetComponent<MazeBuilder>();
    }

    public IEnumerator LinkCells()
    {
        linkingComplete = false;

        // Get random cell
        //MazeCell initialCell = mazeGen.GetRandomCell();
        MazeCell initialCell = mazeGen.mazeData[0, 0];

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

            yield return null;
        }

        linkingComplete = true;
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
}
