using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NavigationState
{
    Idle,
    Navigating,
    Retracing
}

public class Navigate : MonoBehaviour
{
    public NavigationState state;

    private MazeGenerator mazeGenerator;
    private MazeBuilder mazeBuilder;
    public Movement move;
    private DetectWalls detectWalls;

    public bool startedMaze;
    public bool finishedMaze;

    public Vector3 lastPosition;
    public LookDirection lastOriginDirection;

    public bool isInitialised;
    public bool isNavigating;
    public bool isRetracing;
    public bool isTravellingDuringRetrace;
    public bool isIdle;

    public MazeCell currentCell;
    public MazeCell lastCell;
    public Vector2Int currentCellPos;

    public List<MazeCell> visitedCells;

    public FacingDirection facingDirection;

    public int moveCounter;

    private List<MazeCell> decisionPointCells;
    private MazeCell lastDecisionPoint;
    private Dictionary<MazeCell, List<MazeCell>> pathsToDecisionPoints;

    private Stopwatch stopWatch;

    private void Awake()
    {
        mazeGenerator = GameObject.FindGameObjectWithTag("MazeGenerator").GetComponent<MazeGenerator>();
        mazeBuilder = mazeGenerator.GetComponent<MazeBuilder>();
        move = this.GetComponent<Movement>();
        detectWalls = this.GetComponent<DetectWalls>();
        stopWatch = GameObject.FindGameObjectWithTag("Stopwatch").GetComponent<Stopwatch>();

        decisionPointCells = new List<MazeCell>();
        pathsToDecisionPoints = new Dictionary<MazeCell, List<MazeCell>>();
    }

    void Start()
    {
        //Initialise();
    }

    public void Initialise()
    {
        StartCoroutine(InitialiseNavigator());
    }

    private IEnumerator InitialiseNavigator()
    {
        isInitialised = false;

        visitedCells = new List<MazeCell>();

        while (!mazeGenerator.mazeGenerated)
        {
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("Initialising navigator...");

        // Position at start of the maze
        PositionNavigatorAtStartOfMaze();

        lastPosition = this.transform.position;

        // Initialise state of navigation
        state = NavigationState.Navigating;

        startedMaze = true;
        finishedMaze = false;

        currentCellPos = Vector2Int.zero;

        UpdateCellPosition();
        VisitCell(currentCell);        

        yield return new WaitForSeconds(1f);

        stopWatch.ResetStopwatch();
        stopWatch.StartStopwatch();

        move.isMoving = false;
        isTravellingDuringRetrace = false;

        StartCoroutine(TraverseMaze());

        isInitialised = true;
    }

    private void PositionNavigatorAtStartOfMaze()
    {
        mazeGenerator = GameObject.FindGameObjectWithTag("MazeGenerator").GetComponent<MazeGenerator>();

        float x = mazeGenerator.mazeData[0, 0].xPosition;
        float y = 0f;
        float z = mazeGenerator.mazeData[0, 0].yPosition;

        this.transform.position = new Vector3(x, y, z);
    }

    private IEnumerator TraverseMaze()
    {
        while (!isInitialised)
            yield return new WaitForSeconds(0.5f);

        while (startedMaze && !finishedMaze)
        {            
            //Debug.Log($"State: {state}");

            switch (state)
            {
                case NavigationState.Idle: 
                    break;

                case NavigationState.Navigating:
                    if (isNavigating == false)
                    {
                        StartCoroutine(Nav());
                    }
                    break;

                case NavigationState.Retracing:
                    if (isRetracing == false)
                    {
                        StartCoroutine(Retracing());
                    }
                    break;

                default: 
                    break;
            }

            yield return new WaitForSeconds (0.1f);
        }
    }

    private IEnumerator Retracing()
    {
        isRetracing = true;

        // Check for and remove explored neighbour cells
        List<MazeCell> accessibleCells = new List<MazeCell>();

        while (accessibleCells.Count < 1)
        {
            // Move to last decision point cell
            MazeCell lastUnexploredDecisionPoint = decisionPointCells[decisionPointCells.Count - 1];

            isTravellingDuringRetrace = true;
            StartCoroutine(TravelBackToDecisionPoint(lastUnexploredDecisionPoint));
            while (isTravellingDuringRetrace)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Update cell position
            UpdateCellPosition();

            // Get accessible cells
            accessibleCells = currentCell.GetAccessibleUnexploredCells();

            // Remove explored cells
            for (int i = 0; i < accessibleCells.Count; i++)
            {
                if (accessibleCells[i].cellState == CellState.Explored)
                    accessibleCells.Remove(accessibleCells[i]);
            }

            // If no unexplored accessible cells, go to next decision point
            if (accessibleCells.Count == 0)
            {
                Debug.Log("Removing exhausted decision point cell from list");
                decisionPointCells.Remove(currentCell);
                lastDecisionPoint = decisionPointCells[decisionPointCells.Count - 1];
                currentCell.cellState = CellState.Explored;
                ColourCell(currentCell);
            }
        }

        // Return to navigating
        isRetracing = false;
        state = NavigationState.Navigating;
        yield return null;
    }

    private IEnumerator TravelBackToDecisionPoint(MazeCell cell)
    {
        // Get list of cells
        List<MazeCell> cells = pathsToDecisionPoints[cell];

        // Travel through list
        while (currentCell != cell)
        {
            // Target next cell to move to
            MazeCell nextTargetCell = null;
            try
            {
                nextTargetCell = cells[cells.Count - 1];
            }
            catch (System.Exception)
            {
                yield break;
            }

            // Move to cell
            move.RotateNavigator(currentCell, nextTargetCell);
            float timerRotation = 0f;
            while (move.isRotating)
            {
                timerRotation += 0.1f;

                if (timerRotation > move.movementDuration + 0.1f)
                {
                    move.isRotating = false;
                }

                yield return new WaitForSeconds(0.1f);
            }

            StartCoroutine(move.MoveToCell(nextTargetCell));

            // Wait for move to complete
            float timer = 0f;
            while (move.isMoving)
            {
                timer += 0.1f;

                if (timer > move.movementDuration + 0.1f)
                {
                    move.isMoving = false;
                }

                yield return new WaitForSeconds(0.1f);
            }

            // Update cell position
            UpdateCellPosition();

            // Remove cell from list
            cells.Remove(nextTargetCell);
        }

        isTravellingDuringRetrace = false;
    }

    private IEnumerator Nav()
    {
        isNavigating = true;

        while (state == NavigationState.Navigating)
        {
            // Wait for movement to stop
            float timer = 0f;
            while (move.isMoving)
            {
                timer += 0.1f;

                if (timer > move.movementDuration + 0.1f)
                {
                    move.isMoving = false;
                }

                yield return new WaitForSeconds(0.1f);
            }

            // Update cells and coordinates
            UpdateCellPosition();

            // Record visit to cell
            VisitCell(currentCell);

            // Check for solved maze
            CheckForSolvedMaze();

            // Get possible cells to move to
            List<MazeCell> nextCells = currentCell.GetAccessibleUnexploredCells();

            // Remove last visited cell
            if (moveCounter > 0)
            {
                nextCells.Remove(lastCell);
            }

            // Remove explored cells
            for (int i = 0; i < nextCells.Count; i++)
            {
                if (nextCells[i].cellState == CellState.Explored)
                {
                    nextCells.Remove(nextCells[i]);
                }
            }

            // Determine if dead end
            if (nextCells.Count == 0)
            {
                // Dead end, retrace
                currentCell.cellState = CellState.Explored;
                state = NavigationState.Retracing;
            }
            else
            {
                // Remove last cell from next possible cells
                nextCells.Remove(lastCell);
                                
                try 
                {
                    // Choose next cell
                    int randomIndex = Random.Range(0, nextCells.Count);
                    MazeCell nextCell = nextCells[randomIndex];

                    // Move to next cell
                    move.RotateNavigator(currentCell, nextCell);
                    StartCoroutine(move.MoveToCell(nextCell));

                    // Add cell to retrace path
                    pathsToDecisionPoints[lastDecisionPoint].Add(nextCell);

                    // Increment move counter
                    moveCounter++;
                }
                catch (System.Exception)
                {
                    Debug.LogError("Invalid next cell selected");
                }
            }
        }

        isNavigating = false;
    }

    public void CheckForSolvedMaze()
    {
        if (currentCellPos.x == (mazeGenerator.mazeSize.x - 1) &&
            currentCellPos.y == (mazeGenerator.mazeSize.y -1 ))
        {
            // Maze solved
            stopWatch.StopStopwatch();

            Debug.Log("MAZE SOLVED!");
            state = NavigationState.Idle;
            finishedMaze = true;
        }

    }

    public void UpdateCellPosition()
    {
        currentCellPos = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.z)
            );

        lastCell = currentCell;
        currentCell = GetCellByCoordinates(currentCellPos);
    }

    public MazeCell GetCellByCoordinates(Vector2Int position)
    {
        try
        {
            return mazeGenerator.mazeData[position.x, position.y];
        }
        catch (System.Exception)
        {
            Debug.LogError($"ERROR: Invalid cell coordinates [{position.x}, {position.y}]");
            return mazeGenerator.mazeData[0, 0];
        }
    }
    private MazeCell GetNeighbourCellAbsoluteByDirection(FacingDirection facingDirection)
    {
        try
        {
            switch (facingDirection)
            {
                case FacingDirection.North: return mazeGenerator.mazeData[currentCellPos.x, currentCellPos.y + 1];
                case FacingDirection.East: return mazeGenerator.mazeData[currentCellPos.x + 1, currentCellPos.y];
                case FacingDirection.South: return mazeGenerator.mazeData[currentCellPos.x, currentCellPos.y - 1];
                case FacingDirection.West: return mazeGenerator.mazeData[currentCellPos.x - 1, currentCellPos.y];
                default: return currentCell;
            }
        }
        catch (System.Exception) 
        {
            return currentCell;
        }
    }
    void VisitCell(MazeCell cell)
    {
        // Record visit
        cell.visitCount++;

        // Add to visited cells list
        if (!visitedCells.Contains(cell))
        {
            visitedCells.Add(cell);
        }

        // Process cell
        ProcessCell(cell);
    }

    void ProcessCell(MazeCell cell)
    {
        // Count walls
        int wallCounter = 0;
        foreach (var direction in System.Enum.GetValues(typeof(FacingDirection)))
        {
            if (detectWalls.DetectWallInAbsoluteDirection((FacingDirection)direction))
                wallCounter++;
            else
                cell.accessibleDirections.Add((FacingDirection)direction);
        }

        // Determine cell state
        switch (wallCounter)
        {
            case 3:
                cell.cellState = CellState.Explored;
                break;

            case 2:
                if (moveCounter == 0)
                {
                    cell.cellState = CellState.DecisionPoint;
                    decisionPointCells.Add(cell);
                    try
                    {
                        pathsToDecisionPoints.Add(cell, new List<MazeCell>());
                    }
                    catch (System.Exception)
                    {
                        Debug.LogWarning("Mazecell already exists in retrace dictionary");
                    }
                    pathsToDecisionPoints[cell].Add(cell);
                    lastDecisionPoint = cell;
                }
                else
                {
                    cell.cellState = CellState.Explored;
                }
                    
                break;

            case 1:
                cell.cellState = CellState.Explored;

                foreach (var neighbour in cell.GetAccessibleUnexploredCells())
                {
                    if (neighbour.cellState == CellState.Unvisited)
                    {
                        cell.cellState = CellState.DecisionPoint;
                        decisionPointCells.Add(cell);

                        lastDecisionPoint = cell;

                        try
                        {
                            pathsToDecisionPoints.Add(cell, new List<MazeCell>());
                        }
                        catch (System.Exception)
                        {
                            Debug.LogWarning("Mazecell already exists in retrace dictionary");
                        }

                        pathsToDecisionPoints[cell].Add(cell);
                    }
                }              
                break;
        }

        // Colour cell
        ColourCell(cell);
    }

    private void ColourCell(MazeCell cell)
    {
        Material cellMat = null;

        switch (cell.cellState)
        {
            case CellState.Explored:
                cellMat = mazeBuilder.matVisited;
                break;

            case CellState.Unvisited:
                cellMat = mazeBuilder.matUnvisited;
                break;

            case CellState.DecisionPoint:
                cellMat = mazeBuilder.matDecisionPoint;
                break;
        }

        cell.cellDetectorObject.GetComponent<MeshRenderer>().material = cellMat;
    }
}
