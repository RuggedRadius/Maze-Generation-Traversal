using OpenCover.Framework.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class DataSubSet
{
    public DataSubSet(Vector2Int dim, int cnt)
    {
        dimensions = dim;
        testCount = cnt;
    }

    public Vector2Int dimensions;
    public int testCount;
}

public class DataSubSetResults
{
    public float time;
    public int totalCellCount;
    public int visitedCellCount;
}

public class CollectData : MonoBehaviour
{
    public bool isEnabled;

    public List<DataSubSet> dataSubSets;

    private MazeGenerator mazeGenerator;
    private MazeBuilder mazeBuilder;
    private Navigate nav;
    private Stopwatch stopwatch;
    private UIManager ui;

    private bool writingData;

    private string fileName;

    void Start()
    {
        mazeGenerator = GameObject.FindGameObjectWithTag("MazeGenerator").GetComponent<MazeGenerator>();
        mazeBuilder = GameObject.FindGameObjectWithTag("MazeGenerator").GetComponent<MazeBuilder>();
        nav = GameObject.FindGameObjectWithTag("Player").GetComponent<Navigate>();
        stopwatch = GameObject.FindGameObjectWithTag("Stopwatch").GetComponent<Stopwatch>();
        ui = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        string date = $"{DateTime.Now.Date.Year}-{DateTime.Now.Date.Month}-{DateTime.Now.Date.Day}";
        string time = $"{DateTime.Now.TimeOfDay.Hours}-{DateTime.Now.TimeOfDay.Minutes}-{DateTime.Now.TimeOfDay.Seconds}";
        fileName = $"Data_{date}_{time}.csv";
        using (StreamWriter file = new StreamWriter(fileName, true))
        {
            string output = $"Width,Length,Time,Visited Cell Count,Total Cell Count";
            file.WriteLine(output);
        }

        dataSubSets = new List<DataSubSet>();
        //dataSubSets.Add(new DataSubSet(new Vector2Int(2, 2), 5));
        dataSubSets.Add(new DataSubSet(new Vector2Int(5, 5), 10));
        dataSubSets.Add(new DataSubSet(new Vector2Int(10, 10), 10));
        dataSubSets.Add(new DataSubSet(new Vector2Int(20, 20), 10));
        dataSubSets.Add(new DataSubSet(new Vector2Int(30, 30), 10));
        dataSubSets.Add(new DataSubSet(new Vector2Int(40, 40), 10));
        dataSubSets.Add(new DataSubSet(new Vector2Int(50, 50), 10));

        if (isEnabled)
            StartCoroutine(DataCollection());
    }

    public IEnumerator DataCollection()
    {
        Debug.Log("Writing data to file...");

        ui.movementSpeed.value = 1f;

        for (int i = 0; i < dataSubSets.Count; i++)
        {
            DataSubSet currentSet = dataSubSets[i];

            for (int j = 0; j < dataSubSets[i].testCount; j++)
            {
                // Generate maze
                mazeGenerator.GenerateMaze(currentSet.dimensions);

                yield return new WaitForSeconds(1f);

                // Wait for completion of traversal
                while (!nav.finishedMaze)
                {
                    yield return new WaitForSeconds(0.1f);
                }

                yield return new WaitForSeconds(1f);

                // Collect Data
                DataSubSetResults results = new DataSubSetResults();
                results.totalCellCount = mazeBuilder.cellCount;
                results.visitedCellCount = nav.visitedCells.Count;
                results.time = stopwatch.time;

                if (writingData)
                {
                    yield return new WaitForSeconds(0.1f);
                }

                // Output data
                WriteSubSetResultToCsv(currentSet, results);

                yield return null;
            }
        }

        yield return null;

        Debug.Log("Completed data collection!");
    }

    private void WriteSubSetResultToCsv(DataSubSet subset, DataSubSetResults result)
    {
        writingData = true;
        
        using (StreamWriter file = new StreamWriter(fileName, true))
        {
            string output = $"{subset.dimensions.x},{subset.dimensions.y},{result.time},{result.visitedCellCount},{result.totalCellCount}";
            file.WriteLine(output);
        }

        writingData = false;
    }
}
