using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazePathfinder
{
    public delegate void SolutionFinished(bool success);
    public event SolutionFinished OnSolutionFinished;

    MazeGraph mazeGraph;
    MonoBehaviour parent;

    bool isComplete = false;
    
    float startTime = 0;
    int iterations = 0;
    
    public MazePathfinder(MonoBehaviour parent)
    {
        this.parent = parent;
    }

    public void Solve(MazeGraph mazeGraph)
    {
        this.mazeGraph = mazeGraph;

        mazeGraph.exploredNodes = new List<TileNode>();
        mazeGraph.pathNodes = new List<TileNode>();

        mazeGraph.frontierNodes = new PriorityQueue<TileNode>();
        mazeGraph.frontierNodes.Enqueue(mazeGraph.startNode);

        foreach (TileNode t in mazeGraph.mazeNodesList)
            t.Reset();

        isComplete = false;
        iterations = 0;
        mazeGraph.startNode.distanceTraveled = 0;

        parent.StartCoroutine(SearchRoutine());
    }

    public IEnumerator SearchRoutine()
    {
        startTime = Time.realtimeSinceStartup;

        while (!isComplete)
        {
            if (mazeGraph.frontierNodes != null && mazeGraph.frontierNodes.Count > 0)
            {
                TileNode currentNode = mazeGraph.frontierNodes.Dequeue();
                iterations++;

                if (!mazeGraph.exploredNodes.Contains(currentNode))
                {
                    mazeGraph.exploredNodes.Add(currentNode);
                }

                ExpandFrontierAStar(currentNode);

                if (mazeGraph.frontierNodes.Contains(mazeGraph.goalNode))
                {
                    mazeGraph.pathNodes = GetPathNodes(mazeGraph.goalNode);
                    EndAttempt(true);
                }
            }
            else
                EndAttempt(false);
        }

        yield return null;
    }

    void ExpandFrontierAStar(TileNode node)
    {
        if (node == null)
            return;

        for (int i = 0; i < node.neighbors.Length; i++)
        {
            if (node.neighbors[i] != null && !mazeGraph.exploredNodes.Contains(node.neighbors[i]))
            {
                float distanceToNeighbor = mazeGraph.GetManhattanDistance(node, node.neighbors[i]);
                float newDistanceTraveled = distanceToNeighbor + node.distanceTraveled;

                if (float.IsPositiveInfinity(node.neighbors[i].distanceTraveled) || newDistanceTraveled < node.neighbors[i].distanceTraveled)
                {
                    node.neighbors[i].previousNode = node;
                    node.neighbors[i].distanceTraveled = newDistanceTraveled;
                }

                if (!mazeGraph.frontierNodes.Contains(node.neighbors[i]))
                {
                    float hScore = mazeGraph.GetManhattanDistance(node.neighbors[i], mazeGraph.goalNode); //Distance to goal
                    float gScore = node.neighbors[i].distanceTraveled; //Distance to start
                    float fScore = gScore + hScore;

                    node.neighbors[i].priority = fScore;
                    mazeGraph.frontierNodes.Enqueue(node.neighbors[i]);
                }
            }
        }
    }

    void EndAttempt(bool success)
    {
        isComplete = true;
        OnSolutionFinished?.Invoke(success);
    }

    List<TileNode> GetPathNodes(TileNode endNode)
    {
        List<TileNode> path = new List<TileNode>();

        if (endNode == null)
            return path;

        path.Add(endNode);
        TileNode currentNode = endNode.previousNode;

        while (currentNode != null)
        {
            path.Insert(0, currentNode);
            currentNode = currentNode.previousNode;
        }

        return path;
    }

}