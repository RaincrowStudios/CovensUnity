using UnityEngine;
using System.Collections;
using System;
public class ColliderScrollTrigger : MonoBehaviour
{
	public Action<Transform> EnterAction; 
	public Action<Transform> ExitAction; 
	public string tag;
	public bool isChild = true;
	void OnTriggerEnter(Collider other){
		if (other.tag == tag) {
			if(isChild)
			EnterAction (other.transform.GetChild(0));
			else
			EnterAction (other.transform);

		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == tag) {
			if(isChild)
			ExitAction (other.transform.GetChild(0));
			else
			ExitAction (other.transform);
		}
	}
}

