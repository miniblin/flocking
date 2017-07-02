using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {

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
    
    //target for seeking or fleeing
    public Transform target;
    public Transform flee;

    //for path following
    public float pathRadius;
    public List<Transform> pathCheckPoints;

	// Use this for initialization
	void Start () {
        this.rigidbody = GetComponent<Rigidbody>();
        
       
	}
	public void SetBoidArraySize(int size)
    {
        boidArraySize = size;
    }
	
	void Update () {
        Seek(target.position);
         Flee(flee.position);
      //  Wander();
        rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxSpeed);
	}

   
    public void Flee(Vector3 ThreatPosition)
    {
        Vector3 desired = (transform.position-ThreatPosition);
        desired.Normalize();
        desired *= maxSpeed;
        
        Vector3 steer = 0.8f*Vector3.ClampMagnitude((desired - rigidbody.velocity), maxForce);

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
            desired*=(Map(squaredDistance, 0, arrivalRadius, 0, maxForce));
        }
        else
        {
            desired *= maxSpeed;
        }
       
        Vector3 steer = Vector3.ClampMagnitude ((desired - rigidbody.velocity),maxForce);

        
        rigidbody.AddForce(steer);


    }
     

    public void Wander()
    {
        Vector3 predictedPoint =PredictedLocation(100);
        Vector3 randomDirection= predictedPoint+(Random.onUnitSphere.normalized* 30);
        Debug.DrawLine(transform.position, predictedPoint, Color.red);
        Debug.DrawLine(predictedPoint, randomDirection, Color.blue);
        Seek(randomDirection);

    }

    public void FollowPath()
    {
        Vector3 predictedPoint = PredictedLocation(10);
        Vector3 pathTarget = new Vector3(0,0,0);
        float distanceFromNormal = 0;
        float closestNormal = 10000;
        for (int i = 0;i< pathCheckPoints.Count-1; i++)
            {
                Vector3 pathStart = pathCheckPoints[i].position;
                Vector3 pathEnd = pathCheckPoints[i + 1].position;
                
                Vector3 normal = PointOfNormal(transform.position, pathStart, pathEnd);
           
            //check normal is on line
            if (DistanceLineSegmentPoint(pathStart, pathEnd, normal) >0.0000000001) {
                Debug.Log(DistanceLineSegmentPoint(normal, pathStart, pathEnd));
               normal = pathEnd;
            }
            distanceFromNormal = Vector3.Distance(predictedPoint, normal);
            Debug.DrawLine(normal, transform.position, Color.red);
            if (distanceFromNormal < closestNormal)
            {
                closestNormal = distanceFromNormal;
                pathTarget = normal;

            }
            Debug.DrawLine(pathEnd, pathStart, Color.blue);
           



        }
        if (closestNormal > pathRadius)
        {
            Seek(pathTarget);
        }
        Debug.DrawLine(transform.position, pathTarget, Color.green);

    }

    public Vector3 PointOfNormal(Vector3 boidLocation, Vector3 pathStart, Vector3 pathEnd)
    {
        Vector3 path = pathEnd - pathStart;
        Vector3 startToBoid = boidLocation - pathStart;
        path.Normalize();
        path *= Vector3.Dot(startToBoid, path);
        return pathStart + path;
    }

    


    //maps a value from one range to another
    private float Map(float value, float originalMin, float originalMax, float newMin, float newMax)
    {
        return (newMin + (value - originalMin) * (newMax - newMin) / (originalMax - originalMin));
    }

    //return a predicted location of set distance ahead of player
    public Vector3 PredictedLocation(float distance)
    {
        return transform.position+(rigidbody.velocity.normalized * distance);
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

    public void Flock(GameObject[,,] neighbours, int radius, int x, int y, int z, float desiredSeperation)
    {
        this.neighbours = neighbours;
        this.x = x;
        this.y = y;
        this.z = z;
        this.desiredSeperation = desiredSeperation;
        this.neighbourRadius = radius;
        Seperate();
        Alignment();
    }


    public void Seperate()
    {
        
        //average f all fleeing vectors.
        int count = 0;
        Vector3 sumOfFleeVectors=new Vector3(0,0,0);
        
        for (int i = (-neighbourRadius); i <= neighbourRadius; i++)
        {
            for (int j = (-neighbourRadius); j <= neighbourRadius; j++)
            {
                for (int k = (-neighbourRadius); k <= neighbourRadius; k++)
                {
                    if (x + i < boidArraySize && y + j < boidArraySize && z + k < boidArraySize && x + i >= 0 && y + j >= 0 && z + k >= 0)
                    {

                        float d = Vector3.Distance(transform.position, neighbours[x + i, y + j, z + k].transform.position);
                        if (d < desiredSeperation && (d > 0))
                        {
                            Vector3 difference = (transform.position - neighbours[x + i, y + j, z + k].transform.position);
                            difference.Normalize();
                            difference /= d;
                            sumOfFleeVectors += difference;
                            count++;
                        }
                    }

                }
            }
        }


        //sum allthe flee vectors of neighbours cheking all neighbours!!! v slow;
        //foreach (GameObject n in neighbours)
        //{

        //    float d = Vector3.Distance(transform.position, n.transform.position);
        //    if (d < desiredSeperation && (d > 0))
        //    {
        //        Vector3  difference =(transform.position - n.transform.position);
        //        difference.Normalize();
        //        difference /= d;
        //        sumOfFleeVectors += difference;
        //        count++;
        //    }
        //}
       
        //average the flee vectors;
        if (count > 0)
        {
            sumOfFleeVectors /= count;
            sumOfFleeVectors.Normalize();
            sumOfFleeVectors *= (maxSpeed);
            Vector3 steer= Vector3.ClampMagnitude((sumOfFleeVectors - rigidbody.velocity), maxForce);
            rigidbody.AddForce(steer);
        }

    }


    public void Alignment()
    {
        int count = 0;
        Vector3 sumOfNeighbourVeleocities = new Vector3(0, 0, 0);
        float neighbourDistance = 50;
        for (int i = (-neighbourRadius); i <= neighbourRadius; i++)
        {
            for (int j = (-neighbourRadius); j <= neighbourRadius; j++)
            {
                for (int k = (-neighbourRadius); k <= neighbourRadius; k++)
                {
                    if (x + i < boidArraySize && y + j < boidArraySize && z + k < boidArraySize && x + i >= 0 && y + j >= 0 && z + k >= 0)
                    {

                        float d = Vector3.Distance(transform.position, neighbours[x + i, y + j, z + k].transform.position);
                        if (d < neighbourDistance && (d > 0))
                        {

                            sumOfNeighbourVeleocities += neighbours[x + i, y + j, z + k].GetComponent<Rigidbody>().velocity;
                            count++;
                        }
                    }

                }
            }
        }

        if (count > 0)
        {
            sumOfNeighbourVeleocities /= count;
            sumOfNeighbourVeleocities.Normalize();
            sumOfNeighbourVeleocities *= (maxSpeed);
            Vector3 steer = Vector3.ClampMagnitude((sumOfNeighbourVeleocities - rigidbody.velocity), maxForce);
        }
          
    }

     

}
