using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushable : MonoBehaviour
{
    [SerializeField] private Player_Movement _ref;
    private bool moving = false;
    private Vector2 destination;
    private Device device;

    private void Start()
    {
        destination = this.transform.position;
        device = this.gameObject.GetComponent<Device>();
        
        if(device == null)
        {
            Debug.LogError("No device variant script attached");
        }
    }

    void Update()
    {
        if(Vector2.Distance(transform.position, destination) < Mathf.Epsilon)
        {
            transform.position = destination;
            if(moving)
            {
                device.UpdateSources();
            }
            moving = false;
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, _ref.movementSpeed * Time.deltaTime);
            device.UpdateSources();
        }
    }

    public bool push(Vector2 destination)
    {
        if(!moving)
        {
            this.destination = destination;
            moving = true;
            return false;
        }
        return true;
    }
}
