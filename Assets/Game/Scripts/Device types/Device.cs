using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Device : MonoBehaviour
{
    List<Laser> Sources;

    protected void Start()
    {
        Sources = new List<Laser>();
    }

    public void UpdateSources()
    {
        for(int i = 0; i < Sources.Count; i++)
        {
            if (!Sources[i].LaserReflectorMoving(this))
            {
                Sources.Remove(Sources[i]);
            }
        }
    }

    public void HitBy(Laser source, Vector2 direction)
    {
        Laser result = Sources.Find(x => x == source);
        if(result == null)
        {
            Sources.Add(source);
        }
        RedirectLaser(source, direction);
    }

    public abstract void RedirectLaser(Laser source, Vector2 direction);
}
