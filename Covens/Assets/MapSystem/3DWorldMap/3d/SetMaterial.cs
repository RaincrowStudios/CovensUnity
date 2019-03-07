using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMaterial : MonoBehaviour {

	public Material[] mats;
	// Use this for initialization
	void Awake () {
		foreach (Transform item in transform) {
			try {
				item.GetComponent<MeshRenderer>().material = mats [Random.Range (0, mats.Length)];

			} catch (System.Exception ex) {
				
			}
		}
	}
}
