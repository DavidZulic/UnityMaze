using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
public bool visited;

    bool wallEnabledN;
    bool wallEnabledS;
    bool wallEnabledE;
    bool wallEnabledW;

    bool columnEnabledNE;
    bool columnEnabledNW;
    bool columnEnabledSE;
    bool columnEnabledSW;

    public bool[] wallsEnabled;
    bool[] columnsEnabled;
    [SerializeField]
    Transform[] wallTrans;
    [SerializeField]
    Transform[] columnTrans;

    public Vector2Int pos;
    public TileController previousTile;
    
    TileController tileN;
    TileController tileS;
    TileController tileE;
    TileController tileW;

    int neighborsCount;

    public TileNode tileNode;
    public TileView tileView;

    public void Reset(int x, int y)
    {
        pos = new Vector2Int(x, y);
        gameObject.name = "Tile(" + x + "," + y + ")";

        wallEnabledN = true;
        wallEnabledS = true;
        wallEnabledE = true;
        wallEnabledW = true;

        columnEnabledNE = true;
        columnEnabledNW = true;
        columnEnabledSE = true;
        columnEnabledSW = true;

        wallsEnabled = new bool[] { wallEnabledN, wallEnabledS, wallEnabledE, wallEnabledW };
        columnsEnabled = new bool[] { columnEnabledNE, columnEnabledNW, columnEnabledSE, columnEnabledSW };

        visited = false;
        tileView = GetComponent<TileView>();
        tileNode = new TileNode(this, pos);

        previousTile = null;
    }

    public void SetWalls(int wallN = 0, int wallS = 0, int wallE = 0, int wallW = 0)
    {
        wallEnabledN = wallN == 0 ? wallEnabledN : (wallN > 0 ? true : false);
        wallEnabledS = wallS == 0 ? wallEnabledS : (wallS > 0 ? true : false);
        wallEnabledE = wallE == 0 ? wallEnabledE : (wallE > 0 ? true : false);
        wallEnabledW = wallW == 0 ? wallEnabledW : (wallW > 0 ? true : false);

        wallsEnabled = new bool[] { wallEnabledN, wallEnabledS, wallEnabledE, wallEnabledW };
    }

    public void SetNeighbours(TileController tileN, TileController tileS, TileController tileE, TileController tileW)
    {
        this.tileN = tileN;
        this.tileS = tileS;
        this.tileE = tileE;
        this.tileW = tileW;

        TileNode neighborNodeN = wallEnabledN ? null : tileN.tileNode;
        TileNode neighborNodeS = wallEnabledS ? null : tileS.tileNode;
        TileNode neighborNodeE = wallEnabledE ? null : tileE.tileNode;
        TileNode neighborNodeW = wallEnabledW ? null : tileW.tileNode;

        tileNode.SetNeighbors(neighborNodeN, neighborNodeS, neighborNodeE, neighborNodeW);
    }

    public void SetColumns(TileController tileNE, TileController tileNW, TileController tileSE, TileController tileSW)
    {
        //COLUMN NE
        if(tileN != null && tileE != null && tileNE != null)
        {
            columnEnabledNE = wallEnabledN || wallEnabledE || tileN.wallEnabledS || tileN.wallEnabledE || tileE.wallEnabledW || tileE.wallEnabledN || tileNE.wallEnabledS || tileNE.wallEnabledW;
        }
        else
        {
            columnEnabledNE = true;
        }
        //COLUMN NW
        if (tileN != null && tileW != null && tileNW != null)
        {
            columnEnabledNW = wallEnabledN || wallEnabledW || tileN.wallEnabledS || tileN.wallEnabledW || tileW.wallEnabledE || tileW.wallEnabledN || tileNW.wallEnabledS || tileNW.wallEnabledE;
        }
        else
        {
            columnEnabledNW = true;
        }
        //COLUMN SE
        if (tileS != null && tileE != null && tileSE != null)
        {
            columnEnabledSE = wallEnabledS || wallEnabledE || tileS.wallEnabledN || tileS.wallEnabledE || tileE.wallEnabledW || tileE.wallEnabledS || tileSE.wallEnabledN || tileSE.wallEnabledW;
        }
        else
        {
            columnEnabledSE = true;
        }
        //COLUMN SW
        if (tileS != null && tileW != null && tileSW != null)
        {
            columnEnabledSW = wallEnabledS || wallEnabledW || tileS.wallEnabledN || tileS.wallEnabledW || tileW.wallEnabledE || tileW.wallEnabledS || tileSW.wallEnabledN || tileSW.wallEnabledE;
        }
        else
        {
            columnEnabledSW = true;
        }

        columnsEnabled = new bool[] { columnEnabledNE, columnEnabledNW, columnEnabledSE, columnEnabledSW };
    }

    public void UpdateWalls()
    {
        if(wallsEnabled != null && tileView != null)
        {
            tileView.UpdateWalls(wallsEnabled);
        }
    }

    public void UpdateColumns()
    {
        if(columnsEnabled != null && tileView != null)
        {
            tileView.UpdateColumns(columnsEnabled);
        }
    }

    public void SetFloorColor(Color color)
    {
        tileView.SetFloorColor(color);
    }
}

