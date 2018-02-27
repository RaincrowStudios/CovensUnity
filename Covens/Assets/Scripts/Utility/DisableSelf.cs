using UnityEngine;
using System.Collections;

public class DisableSelf : MonoBehaviour
{
	public float timer ;
	// Use this for initialization
	void OnEnable ()
	{
		Invoke ("disableObject", timer);
	}
	
	// Update is called once per frame
	void disableObject ()
	{
		gameObject.SetActive (false);
	}
}

