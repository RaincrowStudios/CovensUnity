using UnityEngine;
using System.Collections;

public class WitchTokenNameControl : MonoBehaviour
{
	public Animator anim;

	void SetupAnim ()
	{
		if (MapsAPI.Instance.zoom > 15) {
			anim.SetBool ("animate", true);
		}else
			anim.SetBool ("animate", false);
	}
}

