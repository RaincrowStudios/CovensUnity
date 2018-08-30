using UnityEngine;
using System.Collections;

public class shiftaudio : MonoBehaviour {
	public float multiplier = 1f;
	int Mid;
	// Use this for initialization
	void Start () {
		Mid = Screen.width / 2;
	}
	
	// Update is called once per frame
	void Update () {
		float k = Input.mousePosition.x - Mid;
		transform.localPosition = new Vector3 (k*multiplier,Input.mousePosition.y*multiplier*.15f,0);
	}
}
