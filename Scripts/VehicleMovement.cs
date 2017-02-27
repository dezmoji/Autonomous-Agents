using UnityEngine;
using System.Collections;

/*
 *Author: Dezmon Gilbert
 *Purpose: To handle the basic movement of agents and hold different steering functions
*/

abstract public class VehicleMovement : MonoBehaviour
{
	// vectors needed for movement
	protected Vector3 acceleration;
	public Vector3 direction;
	public Vector3 velocity;
	protected Vector3 position;
	private Vector3 desired;
	private Vector3 steer;

	// center of terrain
	private Vector3 center;

	// float necessary for forces
	public float mass; // 1
	public float maxSpeed; // varies

	// needed for the lists of objects and weight changes
	protected SceneManager manager;

	// needed for path following
	public GameObject wp;
	protected Path path;
	private Vector3 futurePos;
	public Vector3 closestPoint;
	private float distanceAway;


	// Use this for initialization
	public virtual void Start () 
	{
		// find the manager script
		manager = GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<SceneManager> ();

		// set these to zero
		futurePos = Vector3.zero;
		closestPoint = Vector3.zero;

		// get the path reference for the array
		path = GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<Path> ();

		// find the closest waypoint and set it as the current waypoint to reference
		wp = path.FindClosestWaypoint (gameObject);
	}
	
	// Update is called once per frame
	protected void Update () 
	{
		CalcSteeringForces ();
		UpdatePosition ();
		SetTransform ();	
	}
	
	// update the position of the vehicle
	void UpdatePosition()
	{
		// grab the world position from the transform component
		position = gameObject.transform.position;
		
		// add accel to vel * time
		velocity += acceleration * Time.deltaTime;
		
		// add velocity to position
		position += velocity * Time.deltaTime;
		
		// derive a normalized direction
		direction = velocity.normalized;
		
		// start fresh with new forces every frame
		acceleration = Vector3.zero;
	}
	
	// set the transform compoent to reflect the local position vector
	void SetTransform()
	{
		// setting the y position to terrain y
		position.y = Terrain.activeTerrain.SampleHeight(position) + 1f;
		gameObject.transform.position = position;
		transform.forward = direction;
		gameObject.GetComponent<CharacterController> ().Move (velocity * Time.deltaTime);
	}
	
	// adds to the acceleration
	protected void ApplyForce(Vector3 force)
	{
		//prevent agents from flying
		force.y = 0;
		acceleration += force / mass;
	}
	
	// will determine the steering force for an object to reach a desired location
	protected Vector3 Seek(Vector3 targetPos)
	{
		// find the vector pointing from myself to the target
		desired = targetPos - position;
		
		// scale magnitude to maximum speed to move as quickly as possible
		desired = desired.normalized * maxSpeed;
		
		// find the steering force
		steer = desired - velocity;
		
		// return the steering force
		return steer;
	}
	
	// will determine the steering force for an object to reach a desired location
	protected Vector3 Flee(Vector3 targetPos)
	{
		// find the vector pointing from myself to the target
		desired = targetPos - position;
		
		// scale magnitude to maximum speed to move as quickly as possible
		// make the desired vector point away by multiplying by -1
		desired = desired.normalized * maxSpeed * -1;
		
		// find the steering force
		steer = desired - velocity;
		
		// return the steering force
		return steer;
	}

	// predicts where target will go and then seeks that position
	protected Vector3 Pursue(GameObject target)
	{
		// find the distance between the target and gameobject
		Vector3 distance = target.GetComponent<VehicleMovement>().position - position;	

		// value to scale by to look ahead
		float updatesAhead = distance.magnitude / maxSpeed;

		// get the future position
		Vector3 futurePos = target.GetComponent<VehicleMovement>().position + 
			target.GetComponent<VehicleMovement>().velocity * updatesAhead;

		// seek that position
		return Seek (futurePos);
	}

	// predits where the target will go and then flee that position
	protected Vector3 Evade(GameObject target)
	{
		// find the distance between the target and gameobject
		Vector3 distance = target.GetComponent<VehicleMovement>().position - position;	

		// value to scale by to look ahead
		float updatesAhead = distance.magnitude / maxSpeed;

		// get the future position
		Vector3 futurePos = target.GetComponent<VehicleMovement>().position + 
			target.GetComponent<VehicleMovement>().velocity * updatesAhead;

		// flee that position
		return Flee(futurePos);
	}

	// handles the wandering around of the area
	protected Vector3 Wander()
	{
		// make the center of the circle a normalized velocity vector
		Vector3 circleCenter = velocity.normalized * 3;
		
		// find a random spot within the unit circle and multiply it by a radius of 3f
		Vector2 rndSpot = Random.insideUnitCircle * 1.5f;
		
		// make the random spot on the circle into a Vector3 and and the the center circle to find the desried spot
		desired = new Vector3(rndSpot.x, 0 ,rndSpot.y) + circleCenter;
		
		// find the steering vector
		steer = (desired.normalized * maxSpeed) - velocity;
		
		//return steer
		return steer;
	}

	// handles the alignment of all flockers
	protected Vector3 Alignment()
	{
		// get the average direction and scale it to maxspeed
		desired = manager.avgDir.normalized * maxSpeed;

		// find the steering vector
		steer = desired - velocity;

		return steer;
	}

	// handles alignment for path following
	protected Vector3 Alignment(Vector3 segment)
	{
		// scale the segment to maxspeed
		desired = segment * maxSpeed;
		
		// find the steering vector
		steer = desired - velocity;
		
		return steer;
	}

	// prevents the flockers from getting too close
	protected Vector3 Seperation(GameObject target)
	{
		// flee the target
		// what target will be handled in the indvidual class for each object
		return Flee (target.transform.position);
	}

	// makes the flockers get close together around an average position
	protected Vector3 Cohesion()
	{
		return Seek (manager.avgPos);
	}

	// keeps an agent on a path
	protected Vector3 PathFollowing()
	{
		// find the future position and the closest position
		futurePos = gameObject.transform.position + velocity.normalized * 2f;
		closestPoint = wp.GetComponent<Waypoint>().FindClosestPoint (futurePos);

		// when the agent get close the next way point
		if (Vector3.Distance(gameObject.transform.position, wp.GetComponent<Waypoint>().B) < 3f) 
		{
			// change to the next waypoint
			wp = path.ChangeWaypoint(wp);

			// find the new closest point
			closestPoint = wp.GetComponent<Waypoint>().FindClosestPoint (futurePos);
		}

		// if the projection is greater than 0 and the segment traveled by the agent is larger than the actual segment length
		if (wp.GetComponent<Waypoint> ().projection > 0 && wp.GetComponent<Waypoint>().AB.magnitude < ( wp.GetComponent<Waypoint>().A-
		                                                                                               gameObject.transform.position).magnitude) 
		{
			// change to the next waypoint
			wp = path.ChangeWaypoint(wp);
			
			// find the new closest point
			closestPoint = wp.GetComponent<Waypoint>().FindClosestPoint (futurePos);
		}

		// find the distance from the closest and future positions
		distanceAway = Vector3.Distance(closestPoint,futurePos);

		// if the future position is outside of the radius of the path
		if(distanceAway > 3f)
		{
			// seek the closest point 
			return Seek (closestPoint);
		}

		// otherwise the agent is on the path so return a zero vector
		return Vector3.zero;
	}

	// handles the agent slowing down as it reaches it target
	protected Vector3 Arrival(Vector3 target,float distance)
	{
		return Seek(target) * (distance/5f);
	}

	// all child classes need this
	public abstract void CalcSteeringForces();
}