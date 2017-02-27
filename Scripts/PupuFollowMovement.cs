using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 *Author: Dezmon Gilbert
 *Purpose: To handle movemnt for follower Pupu
*/

public class PupuFollowMovement : VehicleMovement
{	
	// keeps track of neighbors
	public List<GameObject> neighbors;
	
	// needed for weighting and limiting 
	public float seekWeight; // 2
	public float seperationWeight; // 50
	public float maxForce; // 5
	
	public float safeDis; // 3

	// keep track of the distance to leader
	public float distToLeader;

	// Use this for initialization
	public override void Start () 
	{
		base.Start ();
		neighbors = new List<GameObject> ();
	}

	// calculates the steering force
	public override void CalcSteeringForces()
	{
		// find the nearest neighbor
		FindNearestNeighbor ();
		
		// create a new vector
		Vector3 ultimateForce = Vector3.zero;

		// find the distance to the leader's follow position
		distToLeader = Vector3.Distance (gameObject.transform.position, manager.followerSeekPos);

		// if the follower is getting close to the position behind the leader, slow down
		if (distToLeader < 5f) 
		{
			ultimateForce += Arrival (manager.followerSeekPos, distToLeader)* seekWeight;
		} 

		// when the agent gets too close to the leader
		if (distToLeader < 5f) 
		{
			// find the dot product to determine which way to go
			float dotProd = Vector3.Dot(gameObject.transform.position, manager.leaderPu.transform.right);

			// go right
			if(dotProd > 0)
			{
				ultimateForce += Seek((manager.leaderPu.transform.right * 3f) + manager.leaderPu.transform.position);
			}

			// go left
			if(dotProd < 0)
			{
				ultimateForce += Seek((manager.leaderPu.transform.right * -3f) + manager.leaderPu.transform.position);
			}

			// stop moving
			else
			{
				ultimateForce = Vector3.zero;
				ApplyForce(ultimateForce);
				return;
			}
		}

		// otherwise seek the position behind the leader
		else 
		{
			ultimateForce += Seek (manager.followerSeekPos) * seekWeight;
		}

		// if there is anything in the list
		if (neighbors.Count > 0) 
		{
			// apply the seperation for each neighbor
			foreach(GameObject n in neighbors)
			{
				ultimateForce += Seperation(n) * seperationWeight;
			}
		}
		
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
		foreach(GameObject n in manager.followerList)
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
