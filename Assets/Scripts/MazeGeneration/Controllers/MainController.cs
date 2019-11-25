using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GraphView))]
[RequireComponent(typeof(MazeGenerator))]
public class MainController : MonoBehaviour
{
    //TODO: Make variable/expose settings
    const int TILE_SIZE = 11;
    const int MIN_MAZE_SIZE = 3;
    const int MAZE_WIDTH = 33;
    const int MAZE_LENGTH = 33;

    bool mazeGenerated = false;

    //Components
    MazeGenerator mazeGenerator;
    MazePathfinder mazePathfinder;
    MazeGraph mazeGraph;
    GraphView graphView;

    //Settings
    [SerializeField, Range(0, 11)] int mainRoomSize = 3;
    [SerializeField] GameObject tilePrefab = null;
    [SerializeField] Transform mazeRoot = null;

    void Awake()
    {
        mazeGraph = new MazeGraph();
        mazeGenerator = GetComponent<MazeGenerator>();
        mazeGenerator.Initialize(tilePrefab, mazeRoot);

        graphView = GetComponent<GraphView>();

        mazePathfinder = new MazePathfinder(this);
        mazePathfinder.OnSolutionFinished += DisplaySolution;
    }

    private void OnDisable()
    {
        try
        {
            mazePathfinder.OnSolutionFinished -= DisplaySolution;
        }
        catch
        {
            Debug.LogError("Unable to unsubscribe");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            GenerateMaze();
    }

    void GenerateMaze()
    {
        //Generate new random seed
        int mazeSeed = Random.Range(0, System.Int16.MaxValue);

        //Wipe existing maze
        mazeGenerator.Wipe();
        mazeGenerator.SetSeed(mazeSeed);
        mazeGenerator.Spawn(MAZE_WIDTH, MAZE_LENGTH);
        mazeGenerator.ResetTiles();

        if(mainRoomSize > 0)
            mazeGenerator.CarveMainRoom(TILE_SIZE, TILE_SIZE);

        TileController generatorStartTile = mazeGenerator.tiles[0, 0];
        mazeGenerator.FindFirstStep(Direction.N, generatorStartTile.pos);
        mazeGenerator.CleanColumns();
        mazeGenerator.UpdateMeshes();

        mazeGraph.Refresh(mazeGenerator.tiles);
        graphView.ClearNodes(ref mazeGraph.mazeNodesList);

        mazeGenerated = true;

        SolveMaze();
    }

    void SolveMaze()
    {
        if (!mazeGenerated)
            GenerateMaze();

        graphView.ClearNodes(ref mazeGraph.mazeNodesList);
        mazeGraph.startNode = mazeGenerator.tiles[4, 4].tileNode;
        mazeGraph.goalNode = mazeGenerator.tiles[MAZE_WIDTH - 4, MAZE_LENGTH - 4].tileNode;

        graphView.ColorStartNode(mazeGraph.startNode);
        graphView.ColorGoalNode(mazeGraph.goalNode);
        mazePathfinder.Solve(mazeGraph);
    }

    void DisplaySolution(bool success)
    {
        if (!success)
            return;

        graphView.ClearNodes(ref mazeGraph.mazeNodesList);
        graphView.DrawPath(ref mazeGraph.pathNodes, ref mazeGraph.exploredNodes, mazeGraph.startNode, mazeGraph.goalNode);
    }
}