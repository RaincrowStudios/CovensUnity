using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class InventoryScroll : MonoBehaviour  {

	public float count = 30;
	public GameObject item;
	public Transform AnchorPos;
	float step =0;
	public float scrollSpeed = 1;
	public float finalSpeed = 1;
	public float scrollacc = 1;
	public bool canScroll = false;
	bool isRotating = false;
	Vector3 delta = Vector3.zero, lastPos = Vector3.zero;
	// Use this for initialization
	void OnEnable () {

		step = 360 / count;
		print (step);
		for (int i = 0; i < count; i++) {
			var g = Utilities.InstantiateObject (item, AnchorPos);
			g.transform.localEulerAngles = new Vector3 (0, 0, i * step);
			g.transform.GetChild (1).GetComponentInChildren<Text> ().text = "Tool " + i.ToString ();
			g.transform.GetChild (0).GetComponentInChildren<Text> ().text = Random.Range(0,20).ToString();

		}
	}

	void Update()
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
			int dir = (int)delta.y;
			if (delta.y == 0 || isRotating)
				return;
			if (dir > 0) {
				StartCoroutine (RotateWheel (true));
			} else {
				StartCoroutine (RotateWheel (false));
			}
		}
	}

	IEnumerator RotateWheel(bool dir)
	{
		isRotating = true;
		if (dir) {
			transform.localEulerAngles = new Vector3 (0, 0, (step ) + transform.localEulerAngles.z);
		} else {
			transform.localEulerAngles = new Vector3 (0, 0,  transform.localEulerAngles.z - (step ));
		}
		print((step ) + transform.localEulerAngles.z);
		finalSpeed = scrollSpeed ;
		yield return new WaitForSeconds (finalSpeed);

		isRotating = false;
	}
}
