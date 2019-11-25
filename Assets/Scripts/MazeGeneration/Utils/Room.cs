using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public int width;
    public int length;
    public TileController startTile;
    public MazeGenerator mazeGenerator;
    public TileController[,] roomTiles;

    public Room(MazeGenerator mazeGenerator, TileController startTile, int width, int length)
    {
        this.mazeGenerator = mazeGenerator;
        this.startTile = startTile;
        this.width = width;
        this.length = length;

        CarveRoom();
    }
    void CarveRoom()
    {
        roomTiles = new TileController[width, length];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                roomTiles[x, y] = mazeGenerator.tiles[startTile.pos.x + x, startTile.pos.y + y];
                roomTiles[x, y].visited = true;

                if (x == 0)
                {
                    roomTiles[x, y].SetWalls(0, 0, -1, 0);
                }
                else if (x > 0 && x < width - 1)
                {
                    roomTiles[x, y].SetWalls(0, 0, -1, -1);
                }
                else if (x == width - 1)
                {
                    roomTiles[x, y].SetWalls(0, 0, 0, -1);
                }

                if (y == 0)
                {
                    roomTiles[x, y].SetWalls(-1, 0, 0, 0);
                }
                else if (y > 0 && y < length - 1)
                {
                    roomTiles[x, y].SetWalls(-1, -1, 0, 0);
                }
                else if (y == length - 1)
                {
                    roomTiles[x, y].SetWalls(0, -1, 0, 0);
                }
            }
        }

        CarveDoors();
    }

    void CarveDoors()
    {
        int totalDoors = 0;
        // N
        int northDoors = 0;
        bool middleDoorsOnly = Random.Range(0, 12) < 6;
        bool carveDoors = Random.Range(0, 12) < 10;

        if (carveDoors)
        {
            for (int x = 0; x < width; x++)
            {
                int y = length - 1;

                if (middleDoorsOnly)
                {
                    if (x == Mathf.FloorToInt((float)width / 2f))
                    {
                        roomTiles[x, y].SetWalls(-1, 0, 0, 0);
                        mazeGenerator.tiles[startTile.pos.x + x, startTile.pos.y + y + 1].SetWalls(0, -1, 0, 0);
                        northDoors++;
                    }
                }
                else
                {
                    if (x % 2 != 0)
                    {
                        roomTiles[x, y].SetWalls(-1, 0, 0, 0);
                        mazeGenerator.tiles[startTile.pos.x + x, startTile.pos.y + y + 1].SetWalls(0, -1, 0, 0);
                        northDoors++;
                    }
                }
            }
        }

        //S
        totalDoors += northDoors;
        int southDoors = 0;
        middleDoorsOnly = Random.Range(0, 12) < 6;
        carveDoors = Random.Range(0, 12) < 10;

        if (carveDoors)
        {
            for (int x = 0; x < width; x++)
            {
                int y = 0;
                if (middleDoorsOnly)
                {
                    if (x == Mathf.FloorToInt((float)width / 2f))
                    {
                        roomTiles[x, y].SetWalls(0, -1, 0, 0);
                        mazeGenerator.tiles[startTile.pos.x + x, startTile.pos.y + y - 1].SetWalls(-1, 0, 0, 0);
                        southDoors++;
                    }
                }
                else
                {
                    if (x % 2 != 0)
                    {
                        roomTiles[x, y].SetWalls(0, -1, 0, 0);
                        mazeGenerator.tiles[startTile.pos.x + x, startTile.pos.y + y - 1].SetWalls(-1, 0, 0, 0);
                        southDoors++;
                    }
                }
            }
        }

        //E
        totalDoors += southDoors;
        int eastDoors = 0;
        middleDoorsOnly = Random.Range(0, 12) < 6;
        carveDoors = Random.Range(0, 12) < 10;

        if (carveDoors)
        {
            for (int y = 0; y < length; y++)
            {
                int x = width - 1;
                if (middleDoorsOnly)
                {
                    if (y == Mathf.FloorToInt((float)length / 2f))
                    {
                        roomTiles[x, y].SetWalls(0, 0, -1, 0);
                        mazeGenerator.tiles[startTile.pos.x + x + 1, startTile.pos.y + y].SetWalls(0, 0, 0, -1);
                        eastDoors++;
                    }
                }
                else
                {
                    if (y % 2 != 0)
                    {
                        roomTiles[x, y].SetWalls(0, 0, -1, 0);
                        mazeGenerator.tiles[startTile.pos.x + x + 1, startTile.pos.y + y].SetWalls(0, 0, 0, -1);
                        eastDoors++;
                    }
                }
            }
        }

        //W
        totalDoors += eastDoors;
        int westDoors = 0;
        middleDoorsOnly = Random.Range(0, 12) < 6;
        carveDoors = Random.Range(0, 12) < 10;

        if (carveDoors || totalDoors == 0)
        {
            for (int y = 0; y < length; y++)
            {
                int x = 0;
                if (middleDoorsOnly)
                {
                    if (y == Mathf.FloorToInt((float)length / 2f))
                    {
                        roomTiles[x, y].SetWalls(0, 0, 0, -1);
                        mazeGenerator.tiles[startTile.pos.x + x - 1, startTile.pos.y + y].SetWalls(0, 0, -1, 0);
                        westDoors++;
                    }
                }
                else
                {
                    if (y % 2 != 0)
                    {
                        roomTiles[x, y].SetWalls(0, 0, 0, -1);
                        mazeGenerator.tiles[startTile.pos.x + x - 1, startTile.pos.y + y].SetWalls(0, 0, -1, 0);
                        westDoors++;
                    }
                }

            }
        }
        totalDoors += westDoors;
    }
}