using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DELETEME : MonoBehaviour {

	public GameObject WitchStandingIcon;
	public GameObject particle;
	// Use this for initialization
	void Start () {
		LeanTween.moveY (WitchStandingIcon, 200f, 0.01f).setOnComplete(() => Debug.Log("liftin"));
		LeanTween.value (0f, 1f, 3f).setOnComplete(()=> AnimLand());
	}
	
	// Update is called once per frame
	void AnimLand () {
		Debug.Log ("droppin");
		transform.GetChild (0).gameObject.SetActive (true);
		LeanTween.moveY (WitchStandingIcon, 0f, 0.5f).setEase (LeanTweenType.easeOutCubic).setOnComplete (() => {
			//Instantiate(particle, transform);
			Utilities.InstantiateObject(particle, transform);
			LeanTween.value(0f,1f,1f).setOnComplete(() => transform.GetChild (0).gameObject.SetActive (false));
		});
	}
}
