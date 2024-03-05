using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [SerializeField] private Transform movePoint;
    [SerializeField] private Map map;

    public LayerMask stopMovement;
    public float movementSpeed;
    private bool moving = false;
    
    private void Start()
    {
        movePoint.parent = null;
    }
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, movementSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.position, movePoint.position) >= 0.075f)
        {
            map.updateLasers();
            return;
        }
        else if(moving)
        {
            moving = false;
            map.clearUpdateList();
        }

        if(Math.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
        {
            Vector3 newPoint = movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
            if (!checkForObstacles(movePoint.position, new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0)))
            {
                movePoint.position = newPoint;
                map.SetUpdateLaserList(newPoint, new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0));
                moving = true;
            }
        }
        else if (Math.Abs(Input.GetAxisRaw("Vertical")) == 1f)
        {
            Vector3 newPoint = movePoint.position + new Vector3(0, Input.GetAxisRaw("Vertical"), 0);
            if (!checkForObstacles(movePoint.position, new Vector3(0, Input.GetAxisRaw("Vertical"), 0)))
            {
                movePoint.position = newPoint;                //More updates, not only when began moving
                map.SetUpdateLaserList(newPoint, new Vector3(0, Input.GetAxisRaw("Vertical"), 0));
                moving = true;
            }
        }
    }

    public bool checkForObstacles(Vector2 center, Vector2 direction)
    {
        Vector2 destination = center + direction;
        if (Physics2D.OverlapCircle(destination, 0.15f, stopMovement))
        {
            return true;
        }

        Collider2D obstacle = Physics2D.OverlapCircle(destination, 0.15f);
        if (obstacle == null)
        {
            return false;
        }
        else
        {
            if (checkForObstacles(destination, direction))
            {
                return true;
            }
            else
            {
                GameObject obj = obstacle.gameObject;
                destination = destination + direction;
                if(obj.GetComponent<Pushable>().push(destination))
                {
                    //When object is moving prevent movement
                    return true;
                }
                return false;
            }
        }
    }
}
