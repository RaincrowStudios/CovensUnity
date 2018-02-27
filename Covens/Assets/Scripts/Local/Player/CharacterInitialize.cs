using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
public class CharacterInitialize : MonoBehaviour {

	public ObiCloth[] cloth;
	// Use this for initialization
	void OnEnable () {
		var solver = Camera.main.GetComponent<ObiSolver> ();	
		foreach (var item in cloth) {
			var g = item.gameObject;
			item.Solver = solver;	
			var cloth =g.GetComponent<ObiCloth> ().Solver = solver;
			cloth.Initialize ();

		}
//		var cloth = GetComponent<ObiCloth> ().Solver = solver;
//		cloth.Initialize ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
