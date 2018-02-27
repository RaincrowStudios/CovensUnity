using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaPlayerCanvas : MonoBehaviour {

	Vector3 offset;
	public Transform hipJoint;
	Camera cam;
	// Use this for initialization
	void Start () {
		cam = Camera.main;
		offset = transform.position -  hipJoint.position;
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
		cam.transform.rotation * Vector3.up);
		transform.position = hipJoint.transform.position + offset;
	}
}
