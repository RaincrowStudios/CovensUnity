using UnityEngine;
using System.Collections;

public class Disable : MonoBehaviour {

	public float delay = 1f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	Invoke("disable",delay);
	}
	void disable()
	{
		gameObject.SetActive(false);
	}

		
}

