using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Itinerary", menuName = "AirplaneRoute/Itinerary", order = 1)]
public class Itinerary : ScriptableObject
{
    public bool loop = false;
    public int idx = 0;
    public List<Vector3> waypoints = new List<Vector3>();
    public Vector3 origin = Vector3.zero;

    public void SetWaypoints(List<Vector3> waypoints, Vector3 relativeToPosition)
    {
        foreach(Vector3 waypoint in waypoints)
        {
            waypoints.Add(waypoint - origin);
        }
    }
}
