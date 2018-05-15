using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ConditionsManager : MonoBehaviour
{
	public Dictionary<string,Conditions> Conditions = new Dictionary<string, Conditions>();

	bool isClicked = false;
	public Animator anim;

	public void Animate()
	{
		if (!isClicked) {
			anim.SetBool ("animate", true);
			isClicked = true;
		} else {
			close ();
		}
	}

	void close()
	{
		anim.SetBool ("animate", false);
		Invoke ("DisableClick", .4f);
	}
	void DisableClick()
	{
		isClicked = false;
	}
}

