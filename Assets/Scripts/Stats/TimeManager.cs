using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class TimeManager : MonoBehaviour {

    Stopwatch timer;
    Dictionary<Vector3, TimeSpan> points;
    TimeSpan totalTime;

	// Use this for initialization
	void Start () {
        timer = new Stopwatch();
        points = new Dictionary<Vector3, TimeSpan>();
	}
	
	public void StartTimer() {
        timer.Start();
    }

    public void GetPoint(Vector3 g) {
        TimeSpan time = timer.Elapsed;
        points.Add(g, time);
    }

    public void StopTimer() {
        totalTime = timer.Elapsed;
        timer.Stop();
    }

    public String getResults() {
        String results = "";
        foreach (Vector3 g in points.Keys) {
            TimeSpan pointtime = new TimeSpan();
            points.TryGetValue(g, out pointtime);
            results = results + "Point: " + g + ", TimeSpan: " + pointtime.TotalSeconds + "\n";
        }
        results = results + "Total Time: " + totalTime.TotalSeconds + "\n";
        return results;
    }

    public void saveResults() {

    }
}
