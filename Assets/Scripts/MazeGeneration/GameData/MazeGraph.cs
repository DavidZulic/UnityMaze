using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class MazeGraph
{
    int[,] mazeData;
    public int width { get; private set; }
    public int length { get; private set; }

    [HideInInspector] public TileNode[,] mazeNodes;
    [HideInInspector] public List<TileNode> mazeNodesList;

    [HideInInspector] public PriorityQueue<TileNode> frontierNodes;
    [HideInInspector] public List<TileNode> frontierNodesList;
    [HideInInspector] public List<TileNode> exploredNodes;
    [HideInInspector] public List<TileNode> pathNodes;

    [HideInInspector] public TileNode startNode;
    [HideInInspector] public TileNode goalNode;

    public static readonly Vector2Int[] allDirections =
    {
        new Vector2Int(0, 1),   //N
        new Vector2Int(0, -1),  //S
        new Vector2Int(1, 0),   //E
        new Vector2Int(-1, 0)   //W
    };

    public void Refresh(TileController[,] mazeTiles)
    {
        mazeNodesList = new List<TileNode>();

        width = mazeTiles.GetLength(0);
        length = mazeTiles.GetLength(1);

        mazeNodes = new TileNode[width, length];

        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                TileNode newNode = mazeTiles[x, y].tileNode;
                mazeNodes[x,y] = newNode;
                mazeNodesList.Add(newNode);
            }
        }
    }

    public bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < length);
    }

    public float GetManhattanDistance(TileNode source, TileNode target)
    {
        int dx = Mathf.Abs(source.pos.x - target.pos.x);
        int dy = Mathf.Abs(source.pos.y - target.pos.y);

        return dx + dy;
    } 
}
