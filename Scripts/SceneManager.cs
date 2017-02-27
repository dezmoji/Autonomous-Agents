using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 *Author: Dezmon Gilbert
 *Purpose: To handle the scene.
*/

public class SceneManager : MonoBehaviour 
{
	// obtain the correct prefabs
	public GameObject chest;
	public GameObject blobra;
	public GameObject cactuar;
	public GameObject bomb;
	public GameObject pupu;
	public GameObject leader;

	// game objects that are seperate from the prefab
	public GameObject leaderPu;
	public GameObject pathFollower;

	// list to keep track of all gameobjects
	public List<GameObject> flockerList;
	public List<GameObject> followerList;
	public List<GameObject> chestList;

	// needed for flocking
	public Vector3 avgPos;
	public Vector3 avgDir;

	// needed for leader following
	public Vector3 followerSeekPos = Vector3.zero;

	// Use this for initialization
	void Start () 
	{
		// set the initial position of the model
		Vector3 startPos = new Vector3(Random.Range(100f,475f), 0, Random.Range(150f,457f));

		// make sure that the agents starts on the terrain
		startPos.y = Terrain.activeTerrain.SampleHeight(startPos) + .5f; 	

		// instaniate the object
		leaderPu = (GameObject)Instantiate (leader, startPos, Quaternion.identity);

		// create and place the path follower
		startPos = new Vector3(Random.Range(100f,400f), 0, Random.Range(150f,415f));
		startPos.y = Terrain.activeTerrain.SampleHeight(startPos) + .5f;
		pathFollower = (GameObject)Instantiate (cactuar, startPos, Quaternion.identity);

		// set the target for the blobra
		blobra.GetComponent<BlobraMovement> ().target = pathFollower;

		// initialize the lists
		flockerList = new List<GameObject> ();
		chestList = new List<GameObject> ();
		flockerList = new List<GameObject> ();

		// create and place the chests
		for (int i = 0; i < 4; i++) 
		{
			startPos = new Vector3(Random.Range(100f,450f), 0, Random.Range(200f,450f));
			startPos.y = Terrain.activeTerrain.SampleHeight(startPos) + .5f;
			chestList.Add((GameObject)Instantiate (chest, startPos, Quaternion.identity));
		}

		// create and place the leader followers
		for (int i = 0; i < 5; i++) 
		{
			startPos = new Vector3(Random.Range(100f,150f), 0, Random.Range(150f,200f));
			startPos.y = Terrain.activeTerrain.SampleHeight(startPos) + .5f;
			followerList.Add((GameObject)Instantiate (pupu, startPos,  Quaternion.identity));
		}

		// create and place the flockers
		for (int i = 0; i < 5; i++) 
		{
			startPos = new Vector3(Random.Range(250f,300f), 0, Random.Range(250f,315f));
			startPos.y = Terrain.activeTerrain.SampleHeight(startPos) + .5f;
			flockerList.Add((GameObject)Instantiate (blobra, startPos,  Quaternion.identity));
		}

		// set the vectors needed for flocking to zero
		avgDir = Vector3.zero;
		avgPos = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () 
	{	
		FindAveragePosition ();
		FindAverageDirection ();
		FollowerSeekPosition ();
		MoveChest ();
	}

	// will find the average position for a flock
	void FindAveragePosition()
	{
		for (int i = 0; i < flockerList.Count; i++)
		{
			avgPos += flockerList[i].transform.position;
		}

		// divide the avg position
		avgPos /= (flockerList.Count+1);
	}

	// will find the average direction for a flock
	void FindAverageDirection()
	{
		for (int i = 0; i < flockerList.Count; i++)
		{
			avgDir += flockerList[i].transform.forward;
		}
	}

	//find a position behind the leader for the followers to seek
	void FollowerSeekPosition()
	{
		followerSeekPos = leaderPu.transform.position+(leaderPu.GetComponent<VehicleMovement> ().velocity.normalized * -10f);
	}

	// moves the chests around when the pupu get close
	void MoveChest()
	{
		// checks all chests in the list
		foreach (GameObject c in chestList) 
		{
			// if the chest and pupu are too close
			if(Vector3.Distance(c.transform.position,leaderPu.transform.position) < 10f)
			{
				// pick a new position and set it for the chest
				Vector3 newPos = new Vector3(Random.Range(100f,475f), 0, Random.Range(150f,475f));
				newPos.y = Terrain.activeTerrain.SampleHeight(newPos) + .5f;
				c.transform.position = newPos;
			}
		}
	}
}
