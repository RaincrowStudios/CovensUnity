using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour {
	public ParticleSystem stars;
	public float multiplier = 20;
	// Use this for initialization
	void Start () {
		Input.gyro.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		var force = stars.forceOverLifetime;
		force.x = Input.gyro.userAcceleration.x* 50 * multiplier;

	}
}
