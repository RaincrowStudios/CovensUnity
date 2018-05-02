using UnityEngine;
using System.Collections;

public class shiftaudio : MonoBehaviour {
	public float multiplier = 1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	//print (Input.mousePosition);
		transform.localPosition = new Vector3 (Input.mousePosition.x*multiplier,Input.mousePosition.y*multiplier*.15f,0);
	}
}
