using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlantedMirror : Device
{
    private List<Direction.directions> entrances;

    private new void Start()
    {
        base.Start();
        entrances = new List<Direction.directions>();
        entrances.Add(Direction.directions.West);
        entrances.Add(Direction.directions.North);
    }
    public override void RedirectLaser(Laser source, Vector2 direction)
    {
        int index = entrances.FindIndex(x => Direction.getDirectionVector(x) == direction * -1);
        if(index != -1)
        {
            source.castRayTo(this.gameObject.transform.position, Direction.getDirectionVector(entrances[(index + 1) % 2]));
        }
    }
}
