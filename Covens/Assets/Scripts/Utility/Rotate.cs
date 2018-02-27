using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

	public float rotationSpeed = 1f;
	public bool isUp = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(!isUp)
	transform.Rotate (Vector3.forward*Time.deltaTime*rotationSpeed);
		else
			transform.Rotate (Vector3.up*Time.deltaTime*rotationSpeed);

	}
}
