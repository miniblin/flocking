using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSeekAndAvoid : MonoBehaviour {



    public float maxSpeed;
    public float maxForce;
    public float arrivalRadius;
    private Rigidbody rigidbody;
    private int boidArraySize;
    private GameObject[,,] neighbours;
    private int neighbourRadius;

    //this objects x,y,z coords
    private int x;
    private int y;
    private int z;

    //seperation
    private float desiredSeperation;

    //alignment and cohesion
    private float neighbourDistance;

    //target for seeking or fleeing
    public Transform target;
    public Transform flee;

    //for path following
    public float pathRadius;
    public List<Transform> pathCheckPoints;

    // Use this for initialization
    void Start()
    {
        this.rigidbody = GetComponent<Rigidbody>();


    }


    public void SetBoidArraySize(int size)
    {
        boidArraySize = size;
    }

    void Update()
    {
        Seek(target.position);
        Flee(flee.position);
        
        rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxSpeed);
       // transform.LookAt(transform.position + rigidbody.velocity);


    }




    public void Flee(Vector3 ThreatPosition)
    {
        Vector3 desired = (transform.position - ThreatPosition);
        desired.Normalize();
        desired *= maxSpeed;

        Vector3 steer = 0.8f * Vector3.ClampMagnitude((desired - rigidbody.velocity), maxForce);

        rigidbody.AddForce(steer);
    }

    public void Seek(Vector3 target)
    {
        Vector3 desired = (target - transform.position);
        float squaredDistance = desired.sqrMagnitude;

        desired.Normalize();
        //slow the object down as it approaches its destination
        if (squaredDistance < (arrivalRadius * arrivalRadius))
        {
            desired *= (Map(squaredDistance, 0, arrivalRadius, 0, maxForce));
        }
        else
        {
            desired *= maxSpeed;
        }

        Vector3 steer = Vector3.ClampMagnitude((desired - rigidbody.velocity), maxForce);


        rigidbody.AddForce(steer);


    }
    //to remove, used for testing
    public void WanderSeek(Vector3 target)
    {
        Vector3 desired = (target - transform.position);
        float squaredDistance = desired.sqrMagnitude;

        desired.Normalize();
        //slow the object down as it approaches its destination
        if (squaredDistance < (arrivalRadius * arrivalRadius))
        {
            desired *= (Map(squaredDistance, 0, arrivalRadius, 0, maxForce));
        }
        else
        {
            desired *= maxSpeed;
        }

        Vector3 steer = Vector3.ClampMagnitude((desired - rigidbody.velocity), maxForce);


        rigidbody.AddForce(0.6f * steer);


    }


    public void Wander()
    {
        Vector3 predictedPoint = PredictedLocation(100);
        Vector3 randomDirection = predictedPoint + (Random.onUnitSphere.normalized * 30);
        // Debug.DrawLine(transform.position, predictedPoint, Color.red);
        //  Debug.DrawLine(predictedPoint, randomDirection, Color.blue);
        WanderSeek(randomDirection);

    }

    



    //maps a value from one range to another
    private float Map(float value, float originalMin, float originalMax, float newMin, float newMax)
    {
        return (newMin + (value - originalMin) * (newMax - newMin) / (originalMax - originalMin));
    }

    //return a predicted location of set distance ahead of player
    public Vector3 PredictedLocation(float distance)
    {
        return transform.position + (rigidbody.velocity.normalized * distance);
    }

    float DistanceLineSegmentPoint(Vector3 a, Vector3 b, Vector3 p)
    {
        // If a == b line segment is a point and will cause a divide by zero in the line segment test.
        // Instead return distance from a
        if (a == b)
            return Vector3.Distance(a, p);

        // Line segment to point distance equation
        Vector3 ba = b - a;
        Vector3 pa = a - p;
        return (pa - ba * (Vector3.Dot(pa, ba) / Vector3.Dot(ba, ba))).magnitude;
    }

    //https://forum.unity3d.com/threads/how-to-check-a-vector3-position-is-between-two-other-vector3-along-a-line.461474/

    //all boids, a radius to check. and the current boids x,y,z in the array;

    

}



