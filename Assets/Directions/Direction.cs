using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Direction : ScriptableObject
{
    public enum directions {
        North,
        East,
        West,
        South,
        None
    };

    public static Vector2 getDirectionVector(directions dir)
    {
        switch (dir)
        {
            case directions.North:
                return Vector2.up;
            case directions.South:
                return Vector2.down;
            case directions.East:
                return Vector2.right;
            case directions.West:
                return Vector2.left;
            default:
                return Vector2.zero;
        }
    }
}
