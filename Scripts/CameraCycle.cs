using UnityEngine;
using System.Collections;

/*
 * Author: Dezmon Gilbert
 * Purpose: To change the cameras
 * */

public class CameraCycle : MonoBehaviour 
{
	// cameras
	public Camera cam1;
	public Camera cam2;
	public Camera cam3;
	public Camera cam4;
	public Camera cam5;
	public Camera cam6;

	// Camera array that holds a reference to every camera in the scene
	private Camera[] cameras;
	
	// current Camera
	public int currentCameraIndex;

	// string to hold the type of camera
	private string camStr;
	
	// Use this for initialization
	void Start () 
	{
		// initialize the array
		cameras = new Camera[6];

		// set the cameras into the array
		cameras [0] = cam1;
		cameras [1] = cam2;
		cameras [2] = cam3;
		cameras [3] = cam4;
		cameras [4] = cam5;
		cameras [5] = cam6;

		// start the index at 0
		currentCameraIndex = 0;

		// turn all cameras off, except the first default one
		for (int i = 0; i < cameras.Length; i++) 
		{
			cameras[i].gameObject.SetActive(false);
		}

		// if any cameras were added to the controller, enable the first one
		if (cameras.Length > 0) 
		{
			cameras[0].gameObject.SetActive(true);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		// press the 'C' key to cycle through the cameras in the array
		if (Input.GetKeyDown (KeyCode.C)) 
		{
			// cycle to the next camera
			currentCameraIndex++;

			// If cameraIndex is in bounds, set this camera active and last one inactive
			if(currentCameraIndex < cameras.Length)
			{
				cameras[currentCameraIndex-1].gameObject.SetActive(false);
				cameras[currentCameraIndex].gameObject.SetActive(true);
			}

			//if last camera, cycle back to first camera
			else
			{
				cameras[currentCameraIndex-1].gameObject.SetActive(false);
				currentCameraIndex = 0;
				cameras[currentCameraIndex].gameObject.SetActive(true);
			}
		}
	}

	// displays text about the camera
	void OnGUI()
	{
		// store the camera number
		int num = currentCameraIndex + 1;


		// determines which string to display
		if (currentCameraIndex == 0) 
		{
			camStr = "Main";
		}
		if (currentCameraIndex == 1) 
		{
			camStr = "Top-down";
		}
		if (currentCameraIndex == 2) 
		{
			camStr = "From the Mountain";
		}
		if (currentCameraIndex == 3) 
		{
			camStr = "Angled Side 1";
		}
		if (currentCameraIndex == 4) 
		{
			camStr = "Angled Side 2";
		}
		if (currentCameraIndex == 5) 
		{
			camStr = "First Person Controller";
		}

		// display the text
		GUI.Box(new Rect(0,0,200,50),"Press C to change the camera" + "\nCamera " + num + "\n" + camStr );

	}
}
