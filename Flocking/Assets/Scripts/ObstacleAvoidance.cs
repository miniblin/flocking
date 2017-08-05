using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class ObstacleAvoidance : MonoBehaviour

{
    //current vectors of up down left and right raycastes, which will look for a way around obstacles
    private Vector3 lookUp;
    private Vector3 lookDown;
    private Vector3 lookLeft;
    private Vector3 lookRight;

    //current up doen left values within vectors
    float up;
    float down;
    float left;
    float right;
    float z;
    float step = 0.1f;

    Transform transform;
    Rigidbody rigidbody;
    public ObstacleAvoidance(Transform transform, Rigidbody rigidbody)
    {
        this.transform = transform;
        this.rigidbody = rigidbody;
    }

    bool colliding;

    public void AvoidObstacles()
    {
       Vector3 forward = transform.TransformDirection(Vector3.forward) * 20;
      //  Debug.DrawRay(transform.position, forward, Color.green);

        if (Physics.Raycast(transform.position, forward, 10))
            if (FindRoute() != Vector3.zero)
            {
               // print(FindRoute());
                Seek(FindRoute());
            }

    
        
    }

    public void Seek(Vector3 target)
    {
        float maxSpeed = 50f;
        float maxForce = 50f;
        Vector3 desired = (target - transform.position);
        float squaredDistance = desired.sqrMagnitude;

      
        desired.Normalize();
        //slow the object down as it approaches its destination
      
            desired *= maxSpeed;
        

        Vector3 steer = Vector3.ClampMagnitude((desired - rigidbody.velocity), maxForce);

        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(steer);
    }


    private Vector3 FindRoute()
    {

        if (!colliding)
        {
            up = 0;
            down = 0;
            left = 0;
            right = 0;
            z = 1;
            step = 0.1f;
            colliding = true;
        }
        while (colliding)

        {
            if (up <= 1) //while not looking directly up
            {
                lookUp = transform.TransformDirection(new Vector3(0, up += step, z) * 25);
             //   Debug.DrawRay(transform.position, lookUp, Color.red);
                if (!(Physics.Raycast(transform.position, lookUp, 15)))
                {
                    colliding = false;
                   // Debug.DrawRay(transform.position, lookRight, Color.red);
                    return (transform.position + lookUp);
                }
               
            }

            //if (down <= 1) //while not looking directly down
            //{
            //    lookDown = transform.TransformDirection(new Vector3(0, down -= step, z) * 15);
            // //   Debug.DrawRay(transform.position, lookDown, Color.yellow);
            //    if (!(Physics.Raycast(transform.position, lookDown, 10)))
            //    {
            //colliding = false;
            //        return (transform.position + lookDown);
            //    }
            //}

            if (right <= 1) //while not looking directly up
            {
                lookRight = transform.TransformDirection(new Vector3(right+=step, 0, z) * 25);
               // Debug.DrawRay(transform.position, lookRight, Color.blue);
                if (!(Physics.Raycast(transform.position, lookRight, 10)))
                {
                    colliding = false;
                   // Debug.DrawRay(transform.position, lookRight, Color.blue);
                    return (transform.position+lookRight);
                }

            }

            if (left <= 1) //while not looking directly up
            {
                lookLeft = transform.TransformDirection(new Vector3(left-=step, 0, z) * 25);
             //   Debug.DrawRay(transform.position, lookLeft, Color.magenta);
                if (!(Physics.Raycast(transform.position, lookLeft, 10)))
                {
                    colliding = false;
                   // Debug.DrawRay(transform.position, lookRight, Color.magenta);
                    return (transform.position+lookLeft);
                }
                    

            }
            
            z -= step;
            if (z  <= 0)
            {
                colliding = false;
            }
        }
        return Vector3.zero;
    }
}


