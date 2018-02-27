using UnityEngine;
using System.Collections;

public class DisableOBjects : MonoBehaviour {

	public GameObject[] objects;
	// Use this for initialization
	void OnEnable() {
		Invoke ("disableAll", .2f);
	}

	void disableAll()
	{
		foreach (var g in objects) {
			g.SetActive (false);
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
