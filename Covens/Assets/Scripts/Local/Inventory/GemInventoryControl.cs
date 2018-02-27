using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.UI;

public class GemInventoryControl : MonoBehaviour { 
	 
	public float angle= 80;
	// Use this for initialization
	public GameObject Gem;
	public Transform container;
	void OnTriggerEnter(Collider col)
	{
		var c = col.GetComponentInChildren<Image> ().color;
		Destroy (col.gameObject);
		var g = Utilities.InstantiateObject (Gem, container);
		g.transform.localEulerAngles = new Vector3 (0, 0, angle + transform.localEulerAngles.z);
		g.GetComponentInChildren<Image> ().color = c;
	}
}
