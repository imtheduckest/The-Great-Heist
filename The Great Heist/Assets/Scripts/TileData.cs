using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData
{

    public int x;
    public int y;
    public int tileType; // 0 = Floor, 1 = Wall, 2 = Cell Square, 3 = Draw Pile
    public GameObject tile;

    public TileData(int x, int y, int tileType)
    {
        this.x = x;
        this.y = y;
        this.tileType = tileType;
    }
}
