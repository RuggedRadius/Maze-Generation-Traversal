using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Cells Visited Panel")]
    public Text txtVisitedCells;
    public Text txtPercentVisited;

    [Header("Movement Settings Panel")]
    public Slider movementSpeed;
        
    [Header("Camera Settings Panel")]
    public TMP_Dropdown cameraSelection;

    private MazeGenerator mazeGen;
    private MazeBuilder mazeBuilder;
    private Navigate nav;
    private CameraController camController;

    private void Start()
    {
        mazeGen = GameObject.FindGameObjectWithTag("MazeGenerator").GetComponent<MazeGenerator>();
        mazeBuilder = GameObject.FindGameObjectWithTag("MazeGenerator").GetComponent<MazeBuilder>();   
        nav = GameObject.FindGameObjectWithTag("Player").GetComponent<Navigate>();   
        camController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
    }

    void Update()
    {
        DisplayCellCounts();
        UpdateMovementSpeed();
        UpdateCameraSettings();
    }

    public void UpdateCameraSettings()
    {
        switch (cameraSelection.value)
        {
            case 0: // Top Down (Free Move)
                camController.followNavigator = false;
                camController.pointOfViewEnabled = false;
                break;

            case 1: // Top Down (Follow Navigator)
                camController.followNavigator = true;
                camController.pointOfViewEnabled = false;
                break;

            case 2: // Point of View
                camController.followNavigator = false;
                camController.pointOfViewEnabled = true;
                break;
        }
    }

    public void GenerateMaze()
    {
        // Default size
        int width = 10;
        int height = 10;

        // Get size from UI
        Vector2Int mazeSize = new Vector2Int(width, height);
        bool widthSuccess = int.TryParse(GameObject.FindGameObjectWithTag("MazeSizeWidthInput").GetComponent<TMP_InputField>().text, out width);
        bool heightSuccess = int.TryParse(GameObject.FindGameObjectWithTag("MazeSizeHeightInput").GetComponent<TMP_InputField>().text, out height);

        // If parsed correctly, update size
        if (widthSuccess && heightSuccess)
            mazeSize = new Vector2Int(width, height);

        // Generate maze
        mazeGen.GenerateMaze(mazeSize);
    }

    private void UpdateMovementSpeed()
    {
        // Invert value from 0.01 to 1
        float value = 1 - movementSpeed.value;

        // Apply value
        nav.move.movementDuration = value;
    }

    private void DisplayCellCounts()
    {
        try
        {
            // Get counts
            int visitedCells = nav.visitedCells.Count;
            int totalCells = mazeBuilder.cellCount;

            // Calculate percentage
            float percentage = ((float)visitedCells / (float)totalCells) * 100;

            // Update text fields
            txtVisitedCells.text = $"{visitedCells}/{totalCells}";
            txtPercentVisited.text = $"{percentage}%";
        }
        catch (System.Exception) 
        { 
            // Cry
        }
    }
}
