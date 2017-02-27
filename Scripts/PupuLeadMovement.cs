using UnityEngine;
using System.Collections;

/*
 *Author: Dezmon Gilbert
 *Purpose: To handle movemnt for the leader of the Pupu
*/

public class PupuLeadMovement :VehicleMovement
{
	// object for seeking 
	public GameObject target;
	
	// needed for weighting and limiting 
	public float seekWeight; // 10
	public float maxForce; // 5

	// keep track of distance to target
	public float distToTarget;

	// Use this for initialization
	public override void Start () 
	{
		base.Start ();
		target  = null;
	}

	// calculates the steering force
	public override void CalcSteeringForces()
	{
		// find the nearest chest always 
		FindNearestChest();

		// find the distance to the target
		distToTarget = Vector3.Distance (gameObject.transform.position, target.transform.position);

		// create a new vector
		Vector3 ultimateForce = Vector3.zero;

		// seek the target until a certain distance
		if (distToTarget > 3f) 
		{
			ultimateForce += Seek (target.transform.position);
		} 

		// once the target is closer, switch to arrival
		else 
		{
			ultimateForce += Arrival(target.transform.position,3f);
		}

		// clamp the ultimate force
		Vector3.ClampMagnitude (ultimateForce, maxForce);

		// apply the force
		ApplyForce (ultimateForce);
	}

	// method to find the closest humans within a range 
	void FindNearestChest()
	{	
		float closestDist = Mathf.Infinity;

		// iterate through each object in array
		foreach (GameObject c in manager.chestList) 
		{
			// find the distance between current human and zombie
			float distance = Vector3.Distance(gameObject.transform.position, c.transform.position);
			
			// if the distance is less than the safe distance and the game object is not the same
			if(distance < closestDist)
			{
				closestDist = distance;

				target = c;
			}
		}
	}
}
