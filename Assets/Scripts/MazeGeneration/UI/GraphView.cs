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
    [SerializeField] Color blankColor = Color.white;
    [SerializeField] Color startColor = Color.green;
    [SerializeField] Color goalColor = Color.red;
    [SerializeField] Color exploredColor = new Color(0.8f, 0.8f, 0.8f);
    [SerializeField] Color pathColor = Color.cyan;
    
    Coroutine drawCoroutine;

    public void ClearNodes(ref List<TileNode> nodes)
    {
        if (drawCoroutine != null)
            StopCoroutine(drawCoroutine);

        ColorNodesList(ref nodes, blankColor);
    }

    public void ColorStartNode(TileNode node)
    {
        node.SetFloorColor(startColor);
    }

    public void ColorGoalNode(TileNode node)
    {
        node.SetFloorColor(goalColor);
    }

    public void ColorPathNodes(ref List<TileNode> nodes)
    {
        ColorNodesList(ref nodes, pathColor);
    }

    public void ColorExploredNode(TileNode node)
    {
        node.SetFloorColor(exploredColor);
    }

    public void ColorExploredNodes(ref List<TileNode> nodes)
    {
        ColorNodesList(ref nodes, exploredColor);
    }

    /// <summary>
    /// Set each TileNode in nodes List floor color to color
    /// </summary>
    /// <param name="nodes">TileNodes list</param>
    /// <param name="color">TileNode floor color</param>
    public void ColorNodesList(ref List<TileNode> nodes, Color color)
    {
        foreach (TileNode t in nodes)
            t.SetFloorColor(color);
    }

    public void DrawPath(ref List<TileNode> path, ref List<TileNode> steps, TileNode start, TileNode goal)
    {
        if (drawCoroutine != null)
            StopCoroutine(drawCoroutine);

        if (debugLevel == PathDebugLevel.PathOnly)
        {
            ColorPathNodes(ref path);
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
        ColorPathNodes(ref path);

        ColorStartNode(start);
        ColorGoalNode(goal);
        yield return null;
    }
}
