using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TileNode : IComparable<TileNode>
{
    public float priority;

    public Vector2Int pos;

    public TileController controller;

    public TileNode previousNode;
    public TileNode[] neighbors;

    public float distanceTraveled = Mathf.Infinity;

    public TileNode(TileController controller, Vector2Int pos)
    {
        this.controller = controller;
        this.pos = pos;
    }

    public int CompareTo(TileNode other)
    {
        if (priority < other.priority)
        {
            return -1;
        }
        else if (priority > other.priority)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public void SetNeighbors(TileNode neighborN, TileNode neighborS, TileNode neighborE, TileNode neighborW)
    {
        neighbors = new TileNode[] { neighborN, neighborS, neighborE, neighborW};
    }

    public void Reset()
    {
        previousNode = null;
    }
}
