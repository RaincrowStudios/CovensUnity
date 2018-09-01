using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellTraceManager : MonoBehaviour {

	Camera cam;
	public float distancefromcamera = 5;
	// Use this for initialization
	public CastingSound CS;
	public GameObject magic;
	public GameObject magicTrace;
	void Start () {
		cam = GetComponent<Camera>();
	}

	void OnEnable()
	{
		CS.enabled = true;
	} 

	void OnDisable()
	{
		CS.enabled = false;
		if(magicTrace!=null)
		Destroy (magicTrace);

	
	}
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			var targetPos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, distancefromcamera);
			targetPos = cam.ScreenToWorldPoint (targetPos);
			magicTrace = Instantiate (magic, targetPos, Quaternion.identity);
		} 
	
	}
}
