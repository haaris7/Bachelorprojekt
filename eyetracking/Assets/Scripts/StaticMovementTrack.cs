using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticMovementTrack : MonoBehaviour
{
    public GameObject[] Destination;
    private int counter = 0;
    public float rotationspeed = 32f;
    public float speed = 0.5f;
    public float lth = 0.1f;
    private Vector3 Direction;
    public bool IsRotating = true;
    void Start()
    {
        Direction = Destination[counter].transform.position-transform.position;
    }
    void Update()
    {
        Move();
        if(IsRotating == true) Rotate();
    }
    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position,Destination[counter].transform.position,speed*Time.deltaTime);
        if (Vector3.Distance(transform.position, Destination[counter].transform.position) < lth)
        {
            if(counter < Destination.Length-1)
            {
                counter++;
            }
            else
            {
                Destroy(this);
            }
        }
    }
    void Rotate()
    {
        Direction = Destination[counter].transform.position-transform.position;
        Quaternion rot = Quaternion.LookRotation(Direction);
        Quaternion newrot = Quaternion.RotateTowards(transform.rotation, rot, rotationspeed*Time.deltaTime);
        transform.rotation = newrot;
    }
}
