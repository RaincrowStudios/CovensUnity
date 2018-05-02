using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritSelectManager : MonoBehaviour {

	public static SpiritSelectManager Instance { get; set; }
	public GameObject SpiritSelect;
	public Animator anim;

	void Awake()
	{
		Instance = this;
	}

	public void Select()
	{
		SpiritSelect.SetActive (true);
		anim.SetTrigger ("in");
	}

	public void Attack()
	{
		SpiritSelect.SetActive (true);
		anim.SetTrigger ("out");
		Invoke ("disableObject", 1.2f);
		OnPlayerSelect.Instance.OnClick (MarkerSpawner.SelectedMarkerPos, MarkerSpawner.instanceID);


	}

	void disableObject()
	{
		SpiritSelect.SetActive (false);
	}
}
