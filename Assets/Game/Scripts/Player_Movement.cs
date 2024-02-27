using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [SerializeField] private Transform movePoint;

    public LayerMask stopMovement;
    public float movementSpeed;
    
    private void Start()
    {
        movePoint.parent = null;
    }
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, movementSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.position, movePoint.position) >= 0.075f)
        {
            return;
        }

        if(Math.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
        {
            Vector3 newPoint = movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
            if (!checkForObstacles(movePoint.position, new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0)))
            {
                movePoint.position = newPoint;
            }
        }
        else if (Math.Abs(Input.GetAxisRaw("Vertical")) == 1f)
        {
            Vector3 newPoint = movePoint.position + new Vector3(0, Input.GetAxisRaw("Vertical"), 0);
            if (!checkForObstacles(movePoint.position, new Vector3(0, Input.GetAxisRaw("Vertical"), 0)))
            {
                movePoint.position = newPoint;
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
