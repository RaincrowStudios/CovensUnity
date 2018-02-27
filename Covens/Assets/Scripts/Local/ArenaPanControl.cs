using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaPanControl : MonoBehaviour {

	public float xSpeed;
	public float ySpeed;

	public float xSpeedMouse;
	public float ySpeedMouse;

	public Transform cam;
	public Transform camRotControl;
	public ParticleSystem PS;
	public float zoomSpeed;
	public float zoomRotSpeed;
	public float particleColorSpeed;

	public Vector3 velocity;
	public bool _underInertia;
	public float _time = 0.0f;
	public float SmoothTime;

	public float rotSpeed;
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		var main = PS.main;
		if (Input.GetMouseButton (0)) {
			transform.Translate (Input.GetAxis("Mouse X")* Time.deltaTime * xSpeedMouse, 0, Input.GetAxis("Mouse Y")* Time.deltaTime * ySpeedMouse);
		}
		if (Input.touchCount == 2) {
			// Store both touches.
			Touch touchZero = Input.GetTouch (0);
			Touch touchOne = Input.GetTouch (1);

			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			// Otherwise change the field of view based on the change in distance between the touches.
			float change = deltaMagnitudeDiff * Time.deltaTime * zoomSpeed;
			float changeRot = deltaMagnitudeDiff * Time.deltaTime * zoomRotSpeed;

			float camChange = Mathf.Clamp ((cam.transform.localPosition.z + change), -40f, -16f);
			float camRotChange = Mathf.Clamp ((camRotControl.transform.eulerAngles.x - changeRot), 0, 16f);
			cam.transform.localPosition = new Vector3 (0, cam.transform.localPosition.y, camChange);
			camRotControl.transform.eulerAngles = new Vector3 (camRotChange, 0, 0);

			float pChangeAmount = deltaMagnitudeDiff * Time.deltaTime * particleColorSpeed;
			float pChange = Mathf.Clamp ((main.startColor.color.a - pChangeAmount), 0, 0.0235f);
			main.startColor = new Color (1, 1, 1, pChange);
			main.simulationSpeed = 1.5f;

		} else if (Input.touchCount == 1) {
			Touch touchZero = Input.GetTouch (0);

			//print (touchZero.deltaPosition);
			if (touchZero.phase == TouchPhase.Began) {
				_underInertia = false;
			}
			if (touchZero.phase == TouchPhase.Moved) {
				var lastPos = transform.position;
				transform.Translate (touchZero.deltaPosition.x * Time.deltaTime * xSpeed, 0, touchZero.deltaPosition.y * Time.deltaTime * ySpeed);
				velocity = transform.position - lastPos;
			} else if (touchZero.phase == TouchPhase.Ended) {
				_underInertia = true;
			}
			main.simulationSpeed = 1.4f;
		} else if (Input.touchCount == 0 && cam.transform.localPosition.z > -30) {
			main.simulationSpeed = .4f;

		} 

//		else if (Input.touchCount == 3) {
//			Touch t = Input.GetTouch (0);
//			if (t.phase == TouchPhase.Moved) {
//				transform.Rotate (0, t.deltaPosition.y * Time.deltaTime * rotSpeed, 0);
//			}
//		}

		if (_underInertia && _time <= 1) {
			transform.position += velocity;
			velocity = Vector3.Lerp (velocity, Vector3.zero, _time);
			_time += Time.smoothDeltaTime*SmoothTime;
		} else {
			_underInertia = false;
			_time = 0;
		}
	
	}
}
