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

    public void SetWaypoints(List<Vector3> waypoints, Vector3 origin)
    {
        foreach(Vector3 waypoint in waypoints)
        {
            this.waypoints.Add(waypoint - origin);
        }
    }

    public static Itinerary CloneItinerary(Itinerary it)
    {
        Itinerary clone = CreateInstance<Itinerary>();
        clone.SetWaypoints(it.waypoints, Vector3.zero);

        return clone;
    }
}
