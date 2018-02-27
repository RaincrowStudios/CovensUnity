using UnityEngine;
using System.Collections;

public class GemScroll : MonoBehaviour
{
	public bool canScroll = false;
	public float speed= 1;
	Vector3 delta = Vector3.zero, lastPos = Vector3.zero;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonUp (0))
			canScroll = false;

		if (Input.GetMouseButtonDown (0))
			lastPos = Input.mousePosition;
		else if (Input.GetMouseButton (0)) {
			delta = Input.mousePosition - lastPos;
			lastPos = Input.mousePosition;
		}


		if (canScroll) {
			transform.localEulerAngles = new Vector3 (0, 0, (delta.y )*speed + transform.localEulerAngles.z);
		}
	}
}

