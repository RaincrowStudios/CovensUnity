using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnDelay : MonoBehaviour {

	public GameObject[] objects;
	public float delay = 1f;

	// Use this for initialization
	void Start () {
		Invoke ("turnOn", delay);
	}

	void turnOn()
	{
		foreach (var item in objects) {
			item.SetActive (true);
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
}
