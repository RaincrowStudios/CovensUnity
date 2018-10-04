using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTransitionControl : MonoBehaviour {
	
	public static InventoryTransitionControl Instance {get; set;}

	public Animator anim;
	public GameObject InventoryObject;

	void Awake(){
		Instance = this;
	}

	void Start () {

	}

	public void OnAnimateIn ()
	{
		UIStateManager.Instance.CallWindowChanged(false);
		SoundManagerOneShot.Instance.MenuSound ();
		InventoryObject.SetActive (true);
		anim.SetBool ("animate", true);
	}


	public void OnAnimateOut ()
	{
		UIStateManager.Instance.CallWindowChanged(true);
		SoundManagerOneShot.Instance.MenuSound ();
		anim.SetBool ("animate", false);
		Invoke ("disable", .8f);
	}

	void disable()
	{
		InventoryObject.SetActive (false);
		gameObject.SetActive (false);
	}

}
