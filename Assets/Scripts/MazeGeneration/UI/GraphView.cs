using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to set the floor color of the given node/nodes list
/// </summary>
public class GraphView : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] PathDebugLevel debugLevel = PathDebugLevel.PathOnly;
    [Space(10)]
    [SerializeField] Material blankMat;
    [SerializeField] Material startMat;
    [SerializeField] Material goalMat;
    [SerializeField] Material stepMat;
    [SerializeField] Material pathMat;
    
    Coroutine drawCoroutine;

    public void ClearNodes(List<TileNode> nodes)
    {
        if (drawCoroutine != null)
            StopCoroutine(drawCoroutine);

        ColorNodesList(nodes, blankMat);
    }

    public void ColorStartNode(TileNode node)
    {
        ColorNode(node, startMat);
    }

    public void ColorGoalNode(TileNode node)
    {
        ColorNode(node, goalMat);
    }

    public void ColorPathNodes(List<TileNode> nodes)
    {
        ColorNodesList(nodes, pathMat);
    }

    public void ColorExploredNode(TileNode node)
    {
        ColorNode(node, stepMat);
    }

    public void ColorExploredNodes(List<TileNode> nodes)
    {
        ColorNodesList(nodes, stepMat);
    }

    public void ColorNodesList(List<TileNode> nodes, Material mat)
    {
        foreach (TileNode t in nodes)
            t.controller.tileView.SetFloorMaterial(mat);
    }

    public void ColorNode(TileNode node, Material mat)
    {
        node.controller.tileView.SetFloorMaterial(mat);
    }

    public void DrawPath(List<TileNode> path, List<TileNode> steps, TileNode start, TileNode goal)
    {
        if (drawCoroutine != null)
            StopCoroutine(drawCoroutine);

        if (debugLevel == PathDebugLevel.PathOnly)
        {
            ColorPathNodes(path);
            ColorStartNode(start);
            ColorGoalNode(goal);
        }
        else
        {
            drawCoroutine = StartCoroutine(ColorExploredTiles(path, steps, start, goal));
        }
    }

    IEnumerator ColorExploredTiles(List<TileNode> path, List<TileNode> steps, TileNode start, TileNode goal)
    {
        ColorStartNode(start);
        ColorGoalNode(goal);

        foreach (TileNode t in steps)
        {
            ColorExploredNode(t);
            yield return null;
        }

        ColorPathNodes(path);
        ColorStartNode(start);
        ColorGoalNode(goal);
        yield return null;
    }
}
