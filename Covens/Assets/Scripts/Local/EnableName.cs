using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableName : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "MainCamera") {
			transform.GetChild (1).gameObject.SetActive (true);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "MainCamera") {
			transform.GetChild (1).gameObject.SetActive (false);
		}
	}
}
