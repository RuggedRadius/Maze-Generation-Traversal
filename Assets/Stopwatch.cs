using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stopwatch : MonoBehaviour
{
    public bool stopwatchEnabled;
    public float time;

    private Text uiText;

    private void Awake()
    {
        uiText = GameObject.FindGameObjectWithTag("StopwatchUI").GetComponent<Text>();
    }

    public void StartStopwatch()
    {
        stopwatchEnabled = true;
    }

    public void StopStopwatch()
    {
        stopwatchEnabled = false;
    }

    public void ResetStopwatch()
    {
        stopwatchEnabled = false;
        time = 0f;
    }

    void Update()
    {
        if (stopwatchEnabled)
            time += Time.deltaTime;

        DisplayTime();
    }

    private void DisplayTime()
    {
        int milliseconds = (int)TimeSpan.FromSeconds(time).Milliseconds;
        int seconds = (int) TimeSpan.FromSeconds(time).Seconds;
        int minutes = (int)TimeSpan.FromSeconds(time).Minutes;
        int hours = (int)TimeSpan.FromSeconds(time).Hours;

        int totalMilliseconds = (int)TimeSpan.FromSeconds(time).TotalMilliseconds;
        int totalSeconds = (int)TimeSpan.FromSeconds(time).TotalSeconds;
        int totalMinutes = (int)TimeSpan.FromSeconds(time).TotalMinutes;
        int totalHours = (int)TimeSpan.FromSeconds(time).TotalHours;

        uiText.text = $"{hours}:{minutes}:{seconds}:{milliseconds}";


        //int seconds = (int) time;
        //int minutes = (int) (time % 60);
        //int hours = (int) (time % 60) % 60;
    }
}
