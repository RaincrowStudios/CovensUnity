using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillMeImDead : MonoBehaviour {

	public GameObject greyHandOfficFab;
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.F)) {
			Instantiate (greyHandOfficFab, transform);
		}
	}
}
