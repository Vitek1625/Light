using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileCustomData : Tile
{
    public int lightLevel = 0;
    
    public Direction.directions facing = new Direction.directions();
    public int sourceLightLevel = 0;
    public bool isSource = false;
    public bool active = false;

    public TileCustomData copy()
    {
        TileCustomData copy = new TileCustomData();
        copy.lightLevel = this.lightLevel;
        copy.facing = this.facing;
        copy.isSource = this.isSource;
        copy.active = this.active;
        copy.sourceLightLevel = this.sourceLightLevel;
        return copy;
    }
}
