
using UnityEngine;
using System.Collections;

public class MouseFollow : MonoBehaviour 
{
	public Camera cam;
	public float speed = 3.0f;
	private Vector3 targetPos;
	bool isActive = true;
	
	void Start() {
		cam =  GameObject.FindGameObjectWithTag("spellcam").GetComponent<Camera>();
		targetPos = transform.position;    
	}
	
	void Update () {
		
		if (Input.GetMouseButton (0) && isActive) {
			float distance = transform.position.z - cam.transform.position.z;
			targetPos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, distance);
			targetPos = cam.ScreenToWorldPoint (targetPos);
			
			
			transform.position = Vector3.MoveTowards (transform.position, targetPos, speed * Time.deltaTime);
		}
		if (Input.GetMouseButtonUp (0)) {
//			isActive = false;
//			GetComponent<Animator> ().SetBool ("Stop", true);
//			 Destroy (gameObject, 7f);
		}
	}
}