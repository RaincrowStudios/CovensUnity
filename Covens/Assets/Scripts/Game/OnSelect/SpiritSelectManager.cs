using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiritSelectManager : MonoBehaviour {

	public static SpiritSelectManager Instance { get; set; }
	public GameObject SpiritSelect;
	public Animator anim;
	public Button attackButton;

	void Awake()
	{
		Instance = this;
	}

	void OnEnable()
	{
		attackButton.interactable = false;
		EventManager.OnNPCDataReceived += EnableAttack;
	}

	void OnDisable()
	{
		attackButton.interactable = false;
		EventManager.OnNPCDataReceived -= EnableAttack;
	}

	void EnableAttack()
	{
		attackButton.interactable = true;
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
		OnPlayerSelect.Instance.OnClick (MarkerSpawner.SelectedMarkerPos);


	}

	void disableObject()
	{
		SpiritSelect.SetActive (false);
	}
}
