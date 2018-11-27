using UnityEngine;
using System.Collections;

public class TeamManagerUI : MonoBehaviour
{
	public static TeamManagerUI Instance {get;set;}

	public GameObject TeamObject;

	void Awake() {
		Instance = this;
	}

	public void Show() {
		TeamObject.SetActive (true);
	}

}

