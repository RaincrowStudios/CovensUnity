using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour
{

	Camera cam;
	// Use this for initialization
	void Start () {
		cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
			cam.transform.rotation * Vector3.up);
	}
}

