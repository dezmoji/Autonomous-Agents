using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 *Author: Dezmon Gilbert
 *Purpose: To handle movemnt for blobra
*/

public class BlobraMovement : VehicleMovement
{
	// object for seeking 
	public GameObject target;

	// objects for seeking and fleeing
	public List<GameObject> neighbors;
	
	// needed for weighting and limiting 
	public float seekWeight; // 8
	public float alignmentWeight; // 6
	public float cohesionWeight;  // .5
	public float seperationWeight; // 25
	public float maxForce; // 15
	
	public float safeDis; // 20

	// Use this for initialization
	public override void Start () 
	{
		base.Start ();
		neighbors = new List<GameObject> ();
	}
	
	// calculates the steering force
	public override void CalcSteeringForces()
	{
		// find the nearest neighbors
		FindNearestNeighbor ();
		
		// create a new vector
		Vector3 ultimateForce = Vector3.zero;

		// seek a chest
		ultimateForce += Seek (target.transform.position) * seekWeight;

		// make the flockers align
		ultimateForce += Alignment () * alignmentWeight;

		// if there is anything in the list
		if (neighbors.Count > 0) 
		{
			// apply the seperation for each neighbor
			foreach(GameObject n in neighbors)
			{
				ultimateForce += Seperation(n) * seperationWeight;
			}
		}

		// make the flockers get closer together
		ultimateForce += Cohesion () * cohesionWeight;
	
		// clamp the ultimate force
		Vector3.ClampMagnitude (ultimateForce, maxForce);
		
		// apply the forces
		ApplyForce (ultimateForce);
	}
	
	// method to find the closest PuPu within a range 
	void FindNearestNeighbor()
	{	
		// clear the list so there's no over lap
		neighbors.Clear ();
		
		// iterate through each object in array
		foreach(GameObject n in manager.flockerList)
		{
			// find the distance between current human and zombie
			float distance = Vector3.Distance (gameObject.transform.position, n.transform.position);
			
			// if the distance is less than the safe distance and the game object is not the same
			if (distance < safeDis && gameObject != n) 
			{
				// set the target to the closest human
				neighbors.Add (n);
			}
		}
	}
}