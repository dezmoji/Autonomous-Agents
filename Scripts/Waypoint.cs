using UnityEngine;
using System.Collections;

/*
 * Author: Dezmon Gilbert
 * Purpose: To keep important information about a waypoint
 * */

public class Waypoint : MonoBehaviour 
{
	public Vector3 A; // this gameobject
	public Vector3 B; // next gameobject
	public Vector3 AB; // line segment from A to B
	public Vector3 unit; // unit vector of AB

	// needed to access the array in Path
	public Path path;

	// related to the closest point
	public Vector3 AFP;
	public Vector3 closestPoint;
	public float projection;

	// Use this for initialization
	void Start () 
	{
		// get the path reference for the array
		path = GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<Path> ();

		// find the current waypoint and then set the next way point
		for (int i = 0; i < path.pathArray.Length; i++) 
		{
			// if this is the current way point, set it as A
			if (path.pathArray[i] == gameObject)
			{
				A = gameObject.transform.position;

				// if the loop is on the last iteration, set B to the first waypoint in the array
				if(i == 9)
				{
					B = path.pathArray[0].transform.position;
				}

				// otherwise, set B as the next waypoint in the array
				else
				{
					B =path.pathArray[i+1].transform.position;
				}
			}
		}

		// find the segment between A and B
		AB = B-A;

		// find the unit vector of AB
		unit = AB.normalized;
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	// finds the closest point on the line segement relative to the agents future position
	public Vector3 FindClosestPoint(Vector3 futurePos)
	{
		// find the vector from A to the future position
		AFP = futurePos - A;

		// find the projection of AFP onto the line segment
		projection = Vector3.Dot (AFP, unit);

		// find the closest point
		return closestPoint = A + unit * projection;
	}
}
