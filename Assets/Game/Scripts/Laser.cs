using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Laser
{
    private class Relation
    {
        Vector2 pos1;
        Vector2 pos2;

        public Relation(Vector2 pos1, Vector2 pos2)
        {
            this.pos1 = pos1;
            this.pos2 = pos2;
        }

        public Vector2 getPos1()
        {
            return pos1;
        }

        public Vector2 getPos2()
        {
            return pos2;
        }
    }

    Vector2 location;
    Vector2 facing;
    List<RaycastHit2D> Nodes;
    LineRenderer beam;

    List<Relation> Connections;
    LayerMask barrier;
    List<TileLightExtension> Powered;
    // Start is called before the first frame update

    public Laser(LayerMask barrier, Vector2 StartPoint, Vector2 direction, LineRenderer _ref)
    {
        Nodes = new List<RaycastHit2D>();
        Connections = new List<Relation>();
        this.barrier = barrier;
        Powered = new List<TileLightExtension>();
        this.location = StartPoint;
        this.facing = direction;
        Connections = new List<Relation>();

        beam = _ref;
        beam.positionCount = 1;
        beam.SetPosition(0, location);
        //Debug.Log(location);
    }

    public void castRayTo(Vector2 pos, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(pos.x + direction.x/2, pos.y + direction.y/2), direction, Mathf.Infinity, barrier);
        if (hit.collider != null)
        {
            //Debug.Log(hit.collider.gameObject.name);

            //Add relation links
            if(!Nodes.Find(x => GameObject.ReferenceEquals(x.collider.gameObject, hit.collider.gameObject)))
            {
                beam.positionCount++;
                beam.SetPosition(beam.positionCount - 1, hit.point + direction/2);
                Nodes.Add(hit);
            }

            //Check if we hit a wall or an obj on a board
            Device target = hit.collider.gameObject.GetComponent<Device>();

            if(target != null)
            {
                //Debug.Log("Mamy");
                target.HitBy(this, direction);
            }


            //Add Detection of Reciver cell
        }
    }

    public bool LaserReflectorMoving(Device device)
    {
        ResetLasers();
        this.castRayTo(location, facing);
        if (Nodes.Find(x => GameObject.ReferenceEquals(x.collider.gameObject, device.gameObject)))
        {
            return true; //When still hitted by laser
        }
        return false;
    }

    private void ResetLasers()
    {
        Nodes.Clear();
        beam.positionCount = 1;
    }

    public Vector3 getPosition()
    {
        return location;
    }

    public Vector3 getRotation()
    {
        return facing;
    }

    public LineRenderer getBeam()
    {
        return beam;
    }
}
