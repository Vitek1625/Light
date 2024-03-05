using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileLightExtension : TileLight
{
    public Direction.directions facing = Direction.directions.None;
    public int sourceLightLevel = 0;
    public bool activeFromStart = false;
    public bool powered = false;
}
