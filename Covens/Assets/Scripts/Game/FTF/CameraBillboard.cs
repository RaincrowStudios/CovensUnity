using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBillboard : MonoBehaviour {

    public Camera cam;

	// Use this for initialization
	void Start () {
        
        cam = MapsAPI.Instance.camera;
	}
	
	// Update is called once per frame
	void Update () {
        //cam = MapsAPI.Instance.camera;
        transform.rotation = Quaternion.LookRotation(cam.transform.forward, cam.transform.up);
    }
}
