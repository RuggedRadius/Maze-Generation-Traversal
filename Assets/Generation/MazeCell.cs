using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum CellState
{
    Unvisited,
    Explored,
    DecisionPoint
}

namespace Assets
{
    public class MazeCell
    {
        public int xPosition;
        public int yPosition;

        public bool hasVisited;

        public MazeCell neighbourNorth;
        public MazeCell neighbourEast;
        public MazeCell neighbourSouth;
        public MazeCell neighbourWest;

        public MazeWall[] walls;

        public Color cellColour;

        public MazeGenerator mazeGen;

        public GameObject cellObject;
        public GameObject cellDetectorObject;

        public int visitCount;

        public CellState cellState;

        public bool isDecisionPoint;

        public List<FacingDirection> accessibleDirections;

        public MazeCell()
        {
            accessibleDirections = new List<FacingDirection>();
            walls = new MazeWall[4];
            cellState = CellState.Unvisited;
        }

        public List<MazeCell> GetAccessibleUnexploredCells()
        {
            List<MazeCell> cells = new List<MazeCell>();

            foreach (var direction in accessibleDirections)
            {
                MazeCell neighbourCell = null;
                try
                {
                    switch (direction)
                    {
                        case FacingDirection.North:
                            neighbourCell = mazeGen.mazeData[xPosition, yPosition + 1];
                            break;

                        case FacingDirection.East:
                            neighbourCell = mazeGen.mazeData[xPosition + 1, yPosition];
                            break;

                        case FacingDirection.South:
                            neighbourCell = mazeGen.mazeData[xPosition, yPosition - 1];
                            break;

                        case FacingDirection.West:
                            neighbourCell = mazeGen.mazeData[xPosition - 1, yPosition];
                            break;
                    }
                }
                catch (System.Exception) { }

                if (neighbourCell != null &&
                    neighbourCell.cellState != CellState.Explored)
                {
                    cells.Add(neighbourCell);
                }
            }

            return cells;
        }

        public List<MazeCell> GetAccessibleCells()
        {
            List<MazeCell> cells = new List<MazeCell>();

            foreach (var direction in accessibleDirections)
            {
                MazeCell neighbourCell = null;
                try
                {
                    switch (direction)
                    {
                        case FacingDirection.North:
                            neighbourCell = mazeGen.mazeData[xPosition, yPosition + 1];
                            break;

                        case FacingDirection.East:
                            neighbourCell = mazeGen.mazeData[xPosition + 1, yPosition];
                            break;

                        case FacingDirection.South:
                            neighbourCell = mazeGen.mazeData[xPosition, yPosition - 1];
                            break;

                        case FacingDirection.West:
                            neighbourCell = mazeGen.mazeData[xPosition - 1, yPosition];
                            break;
                    }
                }
                catch (System.Exception) {}
                
                if (neighbourCell != null)
                    cells.Add(neighbourCell);
            }

            return cells;
        }

        private Color GetRandomColour()
        {
            float r = UnityEngine.Random.Range(0f, 1f);
            float g = UnityEngine.Random.Range(0f, 1f);
            float b = UnityEngine.Random.Range(0f, 1f);
            float a = 1;

            return new Color(r, g, b, a);
        }

        public bool hasWall(WallFace face)
        {
            switch (face)
            {
                case WallFace.North: return (walls[0] != null);
                case WallFace.East: return (walls[1] != null);
                case WallFace.South: return (walls[2] != null);
                case WallFace.West: return (walls[3] != null);
                default: return false;
            }
        }

        public void DeleteWall(MazeWall wall)
        {
            if (wall.isEdge)
            {
                //Debug.Log($"Skipping [{xPosition}, {yPosition}] {wall.face} wall (edge)");
                return;
            }

            // Delete wall of cell + neighbour side of wall
            switch (wall.face)
            {
                case WallFace.North: 
                    walls[0] = null;
                    break;

                case WallFace.East: 
                    walls[1] = null;
                    break;

                case WallFace.South: 
                    walls[2] = null;
                    break;

                case WallFace.West: 
                    walls[3] = null;
                    break;
            }

            DeleteNeighbourSideWall(wall);

            //Debug.Log($"Removed [{xPosition}, {yPosition}] {wall.face} wall");
        }

        public void DeleteNeighbourSideWall(MazeWall wall)
        {
            try
            {
                switch (wall.face)
                {
                    case WallFace.North:
                        neighbourNorth.walls[2] = null;
                        break;

                    case WallFace.East:
                        neighbourEast.walls[3] = null;
                        break;

                    case WallFace.South:
                        neighbourSouth.walls[0] = null;
                        break;

                    case WallFace.West:
                        neighbourWest.walls[1] = null;
                        break;
                }
            }
            catch (Exception) { }
        }

        public void AddWall(MazeWall newWall)
        {
            switch (newWall.face)
            {
                case WallFace.North:
                    walls[0] = newWall;
                    break;

                case WallFace.East:
                    walls[1] = newWall;
                    break;

                case WallFace.South:
                    walls[2] = newWall;
                    break;

                case WallFace.West:
                    walls[3] = newWall;
                    break;
            }
        }
    }
}
