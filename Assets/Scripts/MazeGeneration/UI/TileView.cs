﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileView : MonoBehaviour
{
    [SerializeField] Transform[] tileWalls = null;
    [SerializeField] Transform[] tileColumns = null;

    [SerializeField] Renderer tileFloorRenderer = null;

    public void SetFloorColor(Color color)
    {
        if (tileFloorRenderer != null)
        {
            tileFloorRenderer.material.color = color;
        }
        else
        {
            Debug.LogError("Tile floor renderer not defined");
            return;
        }
    }

    public void UpdateWalls(bool[] wallsEnabled)
    {
        for (int i = 0; i < wallsEnabled.Length; i++)
        {
            if (tileWalls[i] != null)
            {
                tileWalls[i].gameObject.SetActive(wallsEnabled[i]);
            }
            else
            {
                Debug.LogError("Tile wall: " + i + " not defined");
                return;
            }
        }
    }

    public void UpdateColumns(bool[] columnsEnabled)
    {
        for (int i = 0; i < columnsEnabled.Length; i++)
        {
            if (tileColumns[i] != null)
            {
                tileColumns[i].gameObject.SetActive(columnsEnabled[i]);
            }
            else
            {
                Debug.LogError("Tile column: " + i + " not defined");
                return;
            }
        }
    }

}
