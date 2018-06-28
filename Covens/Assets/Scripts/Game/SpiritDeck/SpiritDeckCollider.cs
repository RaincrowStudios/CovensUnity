using UnityEngine;
using System.Collections;

public class SpiritDeckCollider : MonoBehaviour
{

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "spiritdecktrigger")
		{
			other.GetComponent<Animator>().SetBool("animate",true);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.tag == "spiritdecktrigger")
		{
			other.GetComponent<Animator>().SetBool("animate",false);
		}
	}
}

