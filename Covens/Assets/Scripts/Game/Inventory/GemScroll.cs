using UnityEngine;
using System.Collections;

public class GemScroll : MonoBehaviour
{
	public bool canScroll = false;
	public float speed= 1;
	public float MaxSpeed= 18;

	public float inertia = 3;
	public bool CanRotate;

	float rotateSpeed;
	int direction = 0;

	public Transform[] AllGems;
	// Use this for initialization
	void OnEnable ()
	{
		transform.localEulerAngles = Vector3.zero;
		fixRotationStart ();
	}

	void fixRotationStart()
	{
		foreach (var item in AllGems) {
			item.transform.localEulerAngles = new Vector3 (0, 0, -item.parent.localEulerAngles.z);
		}
	}

	void fixRotation()
	{
		foreach (var item in AllGems) {
			item.Rotate (0, 0, -rotateSpeed*direction );
		}
	}

	public void StartRotation()
	{
		StopAllCoroutines ();
		CanRotate = true;
		StartCoroutine (RotateGemWheel ());
	}

	public void StopRotation()
	{
		CanRotate = false;
		StartCoroutine (RotateGemWheel());
	}

	void Update ()
	{
		if (CanRotate) {
			rotateSpeed = Input.GetAxis ("Mouse Y") * speed;
			if (rotateSpeed > 0)
				direction = 1;
			else
				direction = -1;

			rotateSpeed = Mathf.Clamp(  Mathf.Abs (rotateSpeed), 0, MaxSpeed);
			transform.Rotate (0, 0, rotateSpeed*direction );
			fixRotation ();
		}
	}

	IEnumerator RotateGemWheel()
	{
				while (rotateSpeed>0) {
					rotateSpeed -= Time.deltaTime*inertia;
					transform.Rotate (0, 0, rotateSpeed*direction );
					fixRotation ();
					yield return null;
				} 
			
	}
}

