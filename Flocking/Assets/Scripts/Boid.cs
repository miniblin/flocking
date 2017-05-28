using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {

    public float maxSpeed;
    public float maxForce;
    public float arrivalRadius;
    private Rigidbody rigidbody;
    
    public Transform target;

	// Use this for initialization
	void Start () {
        this.rigidbody = GetComponent<Rigidbody>();
       
	}
	
	// Update is called once per frame
	void Update () {
        Seek(target.position);
        Debug.Log("speed:" + rigidbody.velocity.magnitude);
        rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxSpeed);
	}

   
    public void Flee(Vector3 ThreatPosition)
    {
        Vector3 desired = (transform.position-ThreatPosition);
        desired.Normalize();
        desired *= maxSpeed;
        
        Vector3 steer = Vector3.ClampMagnitude((desired - rigidbody.velocity), maxForce);

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
            desired*=(Map(squaredDistance, 0, arrivalRadius, 0, maxSpeed));
        }
        else
        {
            desired *= maxSpeed;
        }
       
        Vector3 steer = Vector3.ClampMagnitude ((desired - rigidbody.velocity),maxForce);

        
        rigidbody.AddForce(steer);


    }

    //maps a value from one range to another
    private float Map(float value,float originalMin, float originalMax, float newMin, float newMax)
    {
        return (newMin + (value - originalMin) * (newMax - newMin) / (originalMax - originalMin));
    }

    public void Wander()
    {

    }



    public void Flock()
    {

    }

}
