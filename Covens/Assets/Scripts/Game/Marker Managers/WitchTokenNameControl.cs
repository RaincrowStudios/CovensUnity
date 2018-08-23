using UnityEngine;
using System.Collections;

public class WitchTokenNameControl : MonoBehaviour
{
	public Animator anim;
	OnlineMaps OM;
	// Use this for initialization
	void OnEnable ()
	{
		OM = OnlineMaps.instance;
		SetupAnim ();
		OnlineMaps.instance.OnChangeZoom += SetupAnim;

	}

	void OnDisable()
	{
		OnlineMaps.instance.OnChangeZoom -= SetupAnim;
	}
	void OnDestroy()
	{
		try{
		OnlineMaps.instance.OnChangeZoom -= SetupAnim;
		}catch{
			
		}
	}
	// Update is called once per frame
	void SetupAnim ()
	{
		if (OM.zoom > 15) {
			anim.SetBool ("animate", true);
		}else
			anim.SetBool ("animate", false);
	}
}

