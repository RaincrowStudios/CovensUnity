
using UnityEngine;
using System.Collections;

public class Fly : MonoBehaviour 
{

	private Vector3 _screenPoint;
	private Vector3 _offset;
	private Vector3 _curScreenPoint;
	private Vector3 _curPosition;
	private Vector3 _velocity;
	private bool _underInertia;
	private float _time = 0.0f;
	public float SmoothTime;
	public float friction = 1f;

	public float speed = 3.0f;
	private Vector3 targetPos;
	public Camera cam;
	public ParticleSystem[] particles; 
	void Start() {
		//cam =  GameObject.FindGameObjectWithTag("spellcam").GetComponent<Camera>();
		targetPos = transform.position;    

	}



	void Update () {

		if(_underInertia && _time <= SmoothTime)
		{
			transform.position += _velocity;
			_velocity = Vector3.Lerp(_velocity, Vector3.zero, _time);
			_time += Time.smoothDeltaTime;
		}
		else
		{
			_underInertia = false;
			_time = 0.0f;
		}

		if (Input.GetMouseButtonDown (0)) {
			_screenPoint = cam.WorldToScreenPoint(gameObject.transform.position);
			_offset = gameObject.transform.position -cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));
//			Screen.showCursor = false;
			_underInertia = false;
		} 

		if (Input.GetMouseButton (0)) {

			Vector3 _prevPosition = _curPosition;
			_curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
			_curPosition = cam.ScreenToWorldPoint(_curScreenPoint) + _offset;
			_velocity = _curPosition - _prevPosition;
			transform.position = _curPosition;
		} 

		if (Input.GetMouseButtonUp (0)) {

			_underInertia = true;
//			Screen.showCursor = true;
		}
	}


}