using UnityEngine;
using System.Collections;

public class DamageDestroy : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		Invoke ("Remove", 4);
	}


	// Update is called once per frame
	public void Remove () {
		EventManager.Instance.CallArenaDamageInfoTapEvent ();
		ArenaAttackManager.Instance.Damages.Remove (gameObject);
		Destroy (gameObject);
	}
}

