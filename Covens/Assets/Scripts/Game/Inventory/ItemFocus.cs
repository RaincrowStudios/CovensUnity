using UnityEngine;
using System.Collections;

public class ItemFocus : MonoBehaviour
{

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Tool") {
			other.GetComponent<Animator> ().SetBool ("animate", true);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Tool") {
			other.GetComponent<Animator> ().SetBool ("animate", false);
		}
	}
}

