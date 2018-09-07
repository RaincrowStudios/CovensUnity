using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class LocationRuneData : MonoBehaviour
{
	public List<GameObject> spirits = new List<GameObject>();
	public List<SpriteRenderer> players = new List<SpriteRenderer>();

	public GameObject disabled;
	public Button summonButton;

	public void DisableButton(bool canSummon)
	{
		if (!canSummon) {
			summonButton.enabled = false;
			disabled.SetActive (true);
		} else {
			disabled.SetActive (false);
			summonButton.enabled = true;
		}
	}

	public void OnSummon()
	{
		LocationUIManager.Instance.OnSummon ();
	}
}

