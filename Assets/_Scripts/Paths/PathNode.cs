using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PathNode
{
    public PathNode(Vector3 pos, float dist = 0)
    {
        Position = pos;
        Distance = dist;
    }

    public Vector3 Position { get; set; }
    public float Distance { get; set; }
}
