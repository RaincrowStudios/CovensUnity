using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour {
	
	public static TransitionManager Instance {get; set;}

	void Awake(){
		Instance = this;
	}

	void Start () {
		
	}
	
}
