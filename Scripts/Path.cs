using UnityEngine;
using System.Collections;

/*
 * Author: Dezmon Gilbert
 * Purpose: To handle Path related functions and the waypoint array
 * */

public class Path : MonoBehaviour 
{
	// create the array and the waypoints that will be in the array
	public GameObject[] pathArray;
	public GameObject wp0;
	public GameObject wp1;
	public GameObject wp2;
	public GameObject wp3;
	public GameObject wp4;
	public GameObject wp5;
	public GameObject wp6;
	public GameObject wp7;
	public GameObject wp8;
	public GameObject wp9;

	// Use this for initialization before Start
	void Awake()
	{
		// initialize and set the array to specific waypoints
		pathArray = new GameObject[10];
		pathArray [0] = wp0;
		pathArray [1] = wp1;
		pathArray [2] = wp2;
		pathArray [3] = wp3;
		pathArray [4] = wp4;
		pathArray [5] = wp5;
		pathArray [6] = wp6;
		pathArray [7] = wp7;
		pathArray [8] = wp8;
		pathArray [9] = wp9;
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	// will change the way point based on the current way point
	public GameObject ChangeWaypoint(GameObject wp)
	{
		for (int i = 0; i < pathArray.Length; i++) 
		{
			// look for the current Waypoint
			if(wp == pathArray[i])
			{
				// if the Waypoint passed in is the last one, return the first in the array
				if(i == 9)
				{
					return pathArray[0];
				}

				// otherwise, return the next waypoint
				else
				{
					return pathArray[i+1];
				}
			}
		}

		// return the same waypoint to avoid compiling errors
		return wp;
	}

	// find the closest waypoint
	public GameObject FindClosestWaypoint(GameObject obj)
	{
		// set the closest distance to the max
		float closestDist = Mathf.Infinity;

		//set a new Waypoint that will be returned to null
		GameObject wp = null;
		
		// iterate through each object in array
		for(int i = 0; i < pathArray.Length; i++)
		{
			// find the distance between current the waypoints and game object
			float distance = Vector3.Distance(obj.transform.position, pathArray[i].transform.position);
			
			// if the distance is less than the closest distance and the game object is not the same
			if(distance < closestDist)
			{
				// set the new closest distance
				closestDist = distance;

				// set the waypoint
				wp = pathArray[i];
			}
		}

		// return the closest waypoint
		return wp;
	}
}
