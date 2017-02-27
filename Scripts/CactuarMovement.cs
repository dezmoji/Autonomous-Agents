using UnityEngine;
using System.Collections;

/*
 * Author: Dezmon Gilbert
 * Purpose: To handle the steering forces for the cactuar
 * */

public class CactuarMovement: VehicleMovement
{
	// needed for weighting and limiting 
	public float seekWeight; // 3
	public float alignmentWeight; // 1
	public float maxForce; // 10

	// Use this for initialization
	public override void Start () 
	{
		base.Start ();
	}

	// calculates the steering forces
	public override void CalcSteeringForces()
	{
		// create a new vector
		Vector3 ultimateForce = Vector3.zero;

		// wander
		ultimateForce += Wander ();
		//ultimateForce += Seek (wp.GetComponent<Waypoint>().AB);

		// keep alinged to the path
		ultimateForce += Alignment (wp.GetComponent<Waypoint>().unit) * alignmentWeight;

		// keep on the path by calling path following
		ultimateForce += PathFollowing () * seekWeight;

		// clamp the force 
		Vector3.ClampMagnitude (ultimateForce, maxForce);

		// apply the force
		ApplyForce (ultimateForce);
	}
}
