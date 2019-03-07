using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BugsnagUnity;
using BugsnagUnity.Payload;

public class bugsnag_test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Bugsnag.Notify(new System.InvalidOperationException("Test error"));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
