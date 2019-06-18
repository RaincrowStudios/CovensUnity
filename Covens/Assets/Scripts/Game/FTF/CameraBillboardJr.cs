using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBillboardJr : MonoBehaviour
{

    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
       this.transform.rotation = Quaternion.LookRotation(new Vector3(cam.transform.position.x, transform.position.y, cam.transform.position.z), transform.up);
    }
}
