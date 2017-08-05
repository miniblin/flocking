using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {

    public float maxSpeed;
    public float maxForce;
    public float arrivalRadius;

    public Vector3 minBounds;
    public Vector3 maxBounds;
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

    private ObstacleAvoidance obstacleAvoidance;

	// Use this for initialization
	void Start () {
        this.rigidbody = GetComponent<Rigidbody>();
        obstacleAvoidance = new ObstacleAvoidance(transform, rigidbody);
       
	}


	public void SetBoidArraySize(int size)
    {
        boidArraySize = size;
    }
	
	void Update () {
     
       //Seek(target.position);
       // Flee(flee.position);
        Wander();
        rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxSpeed);
        transform.LookAt(transform.position+ rigidbody.velocity);
        obstacleAvoidance.AvoidObstacles();
        CheckBounds();  
        
        
    }

    private void FixedUpdate()
    {
       // CollisionAvoidance();
    }

    
    public void CheckBounds()
    {
        Vector3 desired=rigidbody.velocity;
        if (transform.position.x < minBounds.x)
        {
            desired = (new Vector3(minBounds.x, transform.position.y, transform.position.z) - transform.position);
        }
        if (transform.position.y < minBounds.y)
        {
            desired = (new Vector3(transform.position.x, minBounds.y, transform.position.z) - transform.position);
        }
        if (transform.position.z < minBounds.z)
        {
            desired = (new Vector3(transform.position.x, transform.position.y, minBounds.z) - transform.position);
        }
        if (transform.position.x > maxBounds.x)
        {
            desired = (new Vector3(maxBounds.x, transform.position.y, transform.position.z) - transform.position);
        }
        if (transform.position.y > maxBounds.y)
        {
            desired = (new Vector3(transform.position.x, maxBounds.y, transform.position.z) - transform.position);
        }

        if (transform.position.z > maxBounds.z)
        {
            desired = (new Vector3(transform.position.x, transform.position.y, maxBounds.z) - transform.position);
        }

        
        Vector3 steer = 1.3f * Vector3.ClampMagnitude((desired - rigidbody.velocity), maxForce);

        rigidbody.AddForce(steer);
        //reeverse the thing in the bounds direction
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


        rigidbody.AddForce(0.6f*steer);


    }


    public void Wander()
    {
        Vector3 predictedPoint =PredictedLocation(100);
        Vector3 randomDirection= predictedPoint+(Random.onUnitSphere.normalized* 30);
       // Debug.DrawLine(transform.position, predictedPoint, Color.red);
      //  Debug.DrawLine(predictedPoint, randomDirection, Color.blue);
        WanderSeek(randomDirection);

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

    public void Flock(GameObject[,,] neighbours, int radius, int x, int y, int z, float desiredSeperation, float neighbourDistance)
    {
        this.neighbours = neighbours;
        this.x = x;
        this.y = y;
        this.z = z;
        this.desiredSeperation = desiredSeperation;
        this.neighbourRadius = radius;
        this.neighbourDistance = neighbourDistance;
       Seperate();
       // Alignment();
       // Cohesion();
    }

    int seperationCount = 0;
    Vector3 sumOfFleeVectors = new Vector3(0, 0, 0);

    int cohesionCount = 0;
    Vector3 sumOfNeighborPositions = new Vector3(0, 0, 0);

    int alignmentCount = 0;
    Vector3 sumOfNeighbourVeleocities = new Vector3(0, 0, 0);

    Vector3 difference;
    float d;
    public void Seperate()
    {

        //average  all fleeing vectors of local neighbours.
        seperationCount = 0;
        cohesionCount = 0;
        alignmentCount = 0;

        sumOfFleeVectors = Vector3.zero;
        sumOfNeighborPositions = Vector3.zero;
        sumOfNeighborPositions = Vector3.zero;

        

        for (int i = (-neighbourRadius); i <= neighbourRadius; i++)
        {
            for (int j = (-neighbourRadius); j <= neighbourRadius; j++)
            {
                for (int k = (-neighbourRadius); k <= neighbourRadius; k++)
                {
                    if (x + i < boidArraySize && y + j < boidArraySize && z + k < boidArraySize && x + i >= 0 && y + j >= 0 && z + k >= 0)
                    {

                        //seperation
                        d = Vector3.Distance(transform.position, neighbours[x + i, y + j, z + k].transform.position);
                        if (d < desiredSeperation && (d > 0))
                        {
                            difference = (transform.position - neighbours[x + i, y + j, z + k].transform.position);
                            difference.Normalize();
                            difference /= d;
                            sumOfFleeVectors += difference;
                            seperationCount++;
                        }

                        //cohesion
                        if (d < neighbourDistance && (d > 0))
                        {

                            sumOfNeighborPositions += neighbours[x + i, y + j, z + k].transform.position;
                            cohesionCount++;


                            //alignment

                            sumOfNeighbourVeleocities += neighbours[x + i, y + j, z + k].GetComponent<Rigidbody>().velocity;
                            alignmentCount++;
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
        
        if (seperationCount > 0)
        {
            sumOfFleeVectors /= seperationCount;
            sumOfFleeVectors.Normalize();
            sumOfFleeVectors *= (maxSpeed);
            Vector3 seperationSteer= Vector3.ClampMagnitude((sumOfFleeVectors - rigidbody.velocity), maxForce);
            rigidbody.AddForce(seperationSteer);
        }

        if (cohesionCount > 0)
        {
            sumOfNeighborPositions /= cohesionCount;
            Seek(sumOfNeighborPositions);
        }

        if (alignmentCount > 0)
        {
            sumOfNeighbourVeleocities /= alignmentCount;
            sumOfNeighbourVeleocities.Normalize();
            sumOfNeighbourVeleocities *= (maxSpeed);
            Vector3 steer = Vector3.ClampMagnitude((sumOfNeighbourVeleocities - rigidbody.velocity), maxForce);
            rigidbody.AddForce(0.4f * steer);
        }

    }


    public void Alignment()
    {
        int count = 0;
        Vector3 sumOfNeighbourVeleocities = new Vector3(0, 0, 0);
        float neighbourDistance = 40;
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

    public void StayInBounds(Vector3 min, Vector3 max)
    {
       //if nearing aboundary, turn the fuck around!
       //same direction but neg x,y or z depening onding which boundary 
       // being broken

    }

   // public void CollisionAvoidance()
   // {
       // Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
       // Debug.DrawRay(transform.position, forward, Color.green);

        //seek edge of object
        //do a good avoid

        //after this has been implemented, introduce a ground plane and some sky scrapers
        //make a fuckinig great demo for paul
   // }


    

   
    public void Cohesion()
    {
        int count = 0;
        Vector3 sumOfNeighborPositions = new Vector3(0, 0, 0);
        float neighbourDistance = 40;
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

                            sumOfNeighborPositions += neighbours[x + i, y + j, z + k].transform.position;
                            count++;
                        }
                    }

                }
            }
        }

        if (count > 0)
        {
            sumOfNeighborPositions /= count;
            Seek(sumOfNeighborPositions);
        }

    }

} 

