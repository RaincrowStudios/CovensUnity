using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScrollItemFocus : MonoBehaviour {

	public Text count;
	public Text title;

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "InventoryItem") {
			other.GetComponent<Animator> ().SetBool ("animate", true);
			count.text = Random.Range (1, 20).ToString () + " count";
			title.text = other.GetComponent<Text> ().text;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "InventoryItem") {
			other.GetComponent<Animator> ().SetBool ("animate", false);
		}
	}
}
