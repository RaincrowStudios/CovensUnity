using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour
{

	Camera cam;
	// Use this for initialization
	void Start () {
		cam = GameObject.FindGameObjectWithTag("Respawn").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
			cam.transform.rotation * Vector3.up);
	}
}

