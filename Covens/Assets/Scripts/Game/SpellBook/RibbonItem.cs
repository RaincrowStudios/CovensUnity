using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RibbonItem : MonoBehaviour {

	Animator anim;

	void Awake(){
		anim = GetComponent<Animator> ();
	}
		
	public void setState(bool state){
		anim.SetBool ("open", state);
	}

}