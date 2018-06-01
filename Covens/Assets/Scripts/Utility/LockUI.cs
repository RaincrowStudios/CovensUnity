using UnityEngine;
using System.Collections;

public class LockUI: MonoBehaviour {
	private Vector3 position;
	// Use this for initialization
	void Awake () {
		position = this.gameObject.GetComponent<RectTransform>().position;
	}

	// Update is called once per frame
	void LateUpdate () {
		if(position != this.gameObject.GetComponent<RectTransform>().position)
		{
			this.gameObject.GetComponent<RectTransform>().position = position;
		}
	}
}
