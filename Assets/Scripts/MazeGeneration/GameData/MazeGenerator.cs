using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public TileController[,] tiles;

    bool tilesSpawned;
    int mazeWidth;
    int mazeLength;
    int mainRoomWidth;
    int mainRoomLength;
    Vector2Int midPoint;
    List<TileController> tileSteps;
    
    GameObject tilePrefab;
    Transform mazeRoot;

    public void Initialize(GameObject tilePrefab, Transform mazeRoot)
    {
        this.tilePrefab = tilePrefab;
        this.mazeRoot = mazeRoot;
    }

    public void SetSeed(int seed)
    {
        Random.InitState(seed);
    }

    public void Spawn(int mazeWidth, int mazeLength)
    {
        this.mazeWidth = mazeWidth;
        this.mazeLength = mazeLength;
        tileSteps = new List<TileController>();

        if (!mazeRoot)
            mazeRoot = GameObject.Find("MazeRoot").transform;
        if (!tilesSpawned)
            StartCoroutine("SpawnTiles");
    }

    IEnumerator SpawnTiles()
    {
        tiles = new TileController[mazeWidth, mazeLength];

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeLength; y++)
            {
                tiles[x, y] = Instantiate(tilePrefab, new Vector3(x * 16, 0, y * 16), Quaternion.identity, mazeRoot).GetComponent<TileController>();
            }
        }
        tilesSpawned = true;
        yield return null;
    }

    public void ResetTiles()
    {
        for (int x = 0; x < mazeWidth; x++)
            for (int y = 0; y < mazeLength; y++)
                tiles[x, y].Reset(x, y);
    }

    public void FindMidPoint()
    {
        midPoint = new Vector2Int(Mathf.FloorToInt((float)mazeWidth / 2.0f), Mathf.FloorToInt((float)mazeLength / 2.0f));
    }

    TileController currentTile;
    TileController nextTile;

    public void FindFirstStep(Direction direction, Vector2Int startPosition)
    {
        currentTile = tiles[startPosition.x, startPosition.y];
        nextTile = CheckDirection(direction, startPosition);
        if (nextTile != null)
        {
            nextTile.previousTile = currentTile;
            OpenWalls(direction);
            FindNextStep();
        }
        else
        {
            Debug.LogError("Maze impossible");
        }
    }

    public void FindNextStep()
    {
        tileSteps.Add(currentTile);
        currentTile = nextTile;
        currentTile.visited = true;
        nextTile = null;

        List<Direction> possibleDirections = AllDirections();
        List<Direction> directionsToRemove = new List<Direction>();

        FindImpossibleDirections(ref directionsToRemove, currentTile);
        int directionsToRemoveCount = directionsToRemove.Count;

        if (directionsToRemoveCount > 0)
        {
            for (int i = 0; i < directionsToRemoveCount; i++)
            {
                if (possibleDirections.Contains(directionsToRemove[i]))
                {
                    possibleDirections.Remove(directionsToRemove[i]);
                }
            }
        }

        int possibleDirectionsCount = possibleDirections.Count;

        if (possibleDirectionsCount > 0)
        {
            Direction randomDirection = possibleDirections[Random.Range(0, possibleDirectionsCount)];
            nextTile = CheckDirection(randomDirection, new Vector2Int(currentTile.pos.x, currentTile.pos.y));
            if (nextTile != null)
            {
                nextTile.previousTile = currentTile;
                OpenWalls(randomDirection);
                FindNextStep();
            }
            else
            {
                nextTile = currentTile.previousTile;
                if (nextTile != tiles[0, 0])
                {
                    FindNextStep();
                }
            }
        }
        else
        {

            nextTile = currentTile.previousTile;
            if (nextTile != tiles[0, 0])
            {
                FindNextStep();
            }
        }
    }

    public void CarveMainRoom(int mainRoomWidth, int mainRoomLength)
    {
        FindMidPoint();

        if (mainRoomWidth <= 1 || mainRoomLength <= 1)
        {
            return;
        }

        TileController[,] mainRoomTiles = new TileController[mainRoomWidth, mainRoomLength];

        int widthOffset = midPoint.x - Mathf.FloorToInt((float)mainRoomWidth / 2.0f);
        int lengthOffset = midPoint.y - Mathf.FloorToInt((float)mainRoomLength / 2.0f);

        for (int x = 0; x < mainRoomWidth; x++)
        {
            for (int y = 0; y < mainRoomLength; y++)
            {
                int mainTileX = x + widthOffset;
                int mainTileY = y + lengthOffset;

                mainRoomTiles[x, y] = tiles[mainTileX, mainTileY];
                mainRoomTiles[x, y].visited = true;

                if (x == 0)
                {
                    mainRoomTiles[x, y].SetWalls(0, 0, -1, 0);
                }
                else if (x > 0 && x < mainRoomWidth - 1)
                {
                    mainRoomTiles[x, y].SetWalls(0, 0, -1, -1);
                }
                else if (x == mainRoomWidth - 1)
                {
                    mainRoomTiles[x, y].SetWalls(0, 0, 0, -1);
                }

                if (y == 0)
                {
                    mainRoomTiles[x, y].SetWalls(-1, 0, 0, 0);
                }
                else if (y > 0 && y < mainRoomLength - 1)
                {
                    mainRoomTiles[x, y].SetWalls(-1, -1, 0, 0);
                }
                else if (y == mainRoomLength - 1)
                {
                    mainRoomTiles[x, y].SetWalls(0, -1, 0, 0);
                }

                if (y == mainRoomLength - 1 && (x == Mathf.FloorToInt((float)mainRoomWidth / 2f) || x == 2 || x == mainRoomWidth - 3))
                {
                    mainRoomTiles[x, y].SetWalls(-1, 0, 0, 0);
                    tiles[widthOffset + x, lengthOffset + y + 1].SetWalls(0, -1, 0, 0);
                }

                if (y == 0 && (x == Mathf.FloorToInt((float)mainRoomWidth / 2f) || x == 2 || x == mainRoomWidth - 3))
                {
                    mainRoomTiles[x, y].SetWalls(0, -1, 0, 0);
                    tiles[widthOffset + x, lengthOffset + y - 1].SetWalls(-1, 0, 0, 0);
                }

                if (x == 0 && (y == Mathf.FloorToInt((float)mainRoomLength / 2f) || y == 2 || y == mainRoomLength - 3))
                {
                    mainRoomTiles[x, y].SetWalls(0, 0, 0, -1);
                    tiles[widthOffset + x - 1, lengthOffset + y].SetWalls(0, 0, -1, 0);
                }
                if (x == mainRoomWidth - 1 && (y == Mathf.FloorToInt((float)mainRoomLength / 2f) || y == 2 || y == mainRoomLength - 3))
                {
                    mainRoomTiles[x, y].SetWalls(0, 0, -1, 0);
                    tiles[widthOffset + x + 1, lengthOffset + y].SetWalls(0, 0, 0, -1);
                }
            }
        }
    }

    void OpenWalls(Direction direction)
    {
        switch (direction)
        {
            case Direction.N:
                currentTile.SetWalls(-1, 0, 0, 0);
                nextTile.SetWalls(0, -1, 0, 0);
                break;
            case Direction.S:
                currentTile.SetWalls(0, -1, 0, 0);
                nextTile.SetWalls(-1, 0, 0, 0);
                break;
            case Direction.E:
                currentTile.SetWalls(0, 0, -1, 0);
                nextTile.SetWalls(0, 0, 0, -1);
                break;
            case Direction.W:
                currentTile.SetWalls(0, 0, 0, -1);
                nextTile.SetWalls(0, 0, -1, 0);
                break;
        }
    }

    public void CleanColumns()
    {
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeLength; y++)
            {
                TileController tileN = null;
                TileController tileS = null;
                TileController tileE = null;
                TileController tileW = null;
                TileController tileNE = null;
                TileController tileNW = null;
                TileController tileSE = null;
                TileController tileSW = null;

                if (y < mazeLength - 1)
                {
                    tileN = tiles[x, y + 1];
                    if (x < mazeWidth - 1)
                    {
                        tileNE = tiles[x + 1, y + 1];
                    }
                    if (x > 0)
                    {
                        tileNW = tiles[x - 1, y + 1];
                    }
                }

                if (y > 0)
                {
                    tileS = tiles[x, y - 1];
                    if (x < mazeWidth - 1)
                    {
                        tileSE = tiles[x + 1, y - 1];
                    }
                    if (x > 0)
                    {
                        tileSW = tiles[x - 1, y - 1];
                    }
                }

                if (x < mazeWidth - 1)
                {
                    tileE = tiles[x + 1, y];
                }

                if (x > 0)
                {
                    tileW = tiles[x - 1, y];
                }
                tiles[x, y].SetNeighbours(tileN, tileS, tileE, tileW);
                tiles[x, y].SetColumns(tileNE, tileNW, tileSE, tileSW);
            }
        }
    }

    public void UpdateMeshes()
    {
        StartCoroutine(UpdateWalls(0, 0, mazeWidth, mazeLength));
        StartCoroutine(UpdateColumns(0, 0, mazeWidth, mazeLength));
    }

    public void Wipe()
    {
        tileSteps = null;
    }
    
    public IEnumerator UpdateWalls(int xStartIndex, int yStartIndex, int xCount, int yCount)
    {
        yield return null;
        for (int x = xStartIndex; x < xCount; x++)
        {
            for (int y = yStartIndex; y < yCount; y++)
            {
                tiles[x, y].UpdateWalls();
            }
        }
    }

    public IEnumerator UpdateColumns(int xStartIndex, int yStartIndex, int xCount, int yCount)
    {
        yield return null;
        for (int x = xStartIndex; x < xCount; x++)
        {
            for (int y = yStartIndex; y < yCount; y++)
            {
                tiles[x, y].UpdateColumns();
            }
        }
    }
    
    IEnumerator RemoveTiles()
    {
        yield return null;
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                GameObject.Destroy(tiles[x, y].gameObject);
            }
        }
    }

    TileController CheckDirection(Direction direction, Vector2Int position)
    {
        switch (direction)
        {
            case Direction.N:
                if (position.y + 1 < mazeLength)
                {
                    return tiles[position.x, position.y + 1].visited ? null : tiles[position.x, position.y + 1];
                }
                else
                {
                    return null;
                }
            case Direction.S:
                if (position.y - 1 >= 0)
                {
                    return tiles[position.x, position.y - 1].visited ? null : tiles[position.x, position.y - 1];
                }
                else
                {
                    return null;
                }
            case Direction.E:
                if (position.x + 1 < mazeWidth)
                {
                    return tiles[position.x + 1, position.y].visited ? null : tiles[position.x + 1, position.y];
                }
                else
                {
                    return null;
                }
            case Direction.W:
                if (position.x - 1 >= 0)
                {
                    return tiles[position.x - 1, position.y].visited ? null : tiles[position.x - 1, position.y];
                }
                else
                {
                    return null;
                }
        }
        return null;
    }


    /// <summary>
    /// Check if tile in selected direction exists and if it was visited
    /// </summary>
    /// <param name="list">List of impossible tiles</param>
    /// <param name="tile">Current tile</param>
    void FindImpossibleDirections(ref List<Direction> list, TileController tile)
    {
        if ((tile.pos.y + 1 > mazeLength - 1) || (tiles[tile.pos.x, tile.pos.y + 1].visited))
            list.Add(Direction.N);
        if ((tile.pos.y - 1 < 0) || (tiles[tile.pos.x, tile.pos.y - 1].visited))
            list.Add(Direction.S);
        if ((tile.pos.x + 1 > mazeWidth - 1) || (tiles[tile.pos.x + 1, tile.pos.y].visited))
            list.Add(Direction.E);
        if ((tile.pos.x - 1 < 0) || (tiles[tile.pos.x - 1, tile.pos.y].visited))
            list.Add(Direction.W);
    }

    List<Direction> AllDirections()
    {
        List<Direction> directions = new List<Direction>();
        directions.Add(Direction.N);
        directions.Add(Direction.S);
        directions.Add(Direction.E);
        directions.Add(Direction.W);
        return directions;
    }
}