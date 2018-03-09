using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollManagerHerb : MonoBehaviour {

	public static ScrollManagerHerb Instance {get; set;}
	public GameObject item;
	float step =0;
	public int ItemCount = 30;
	public int activeItem = 10;
	public Transform container;

	public bool canScroll = false;
	public float speed= 1;
	public float MaxSpeed= 18;

	public float inertia = 3;
	public bool CanRotate;

	float rotateSpeed;
	int direction = 0;

	public float fixSpeed = 2;

	public List<Transform> allItems = new List<Transform>();
	// Use this for initialization
	Quaternion restAngle;
	public float clampAngle;

	void Awake(){
		Instance = this;
	}

	void Start () {
		step = 360 / ItemCount;
		clampAngle = -(activeItem - 1) * step + 360;
		foreach (Transform item in container) {
			allItems.Clear ();
			Destroy (item.gameObject);
		}

		for (int i = 0; i < ItemCount; i++) {
			var g = Utilities.InstantiateObject (item, container);
			g.GetComponent<HerbItemManager> ().itemName = "Herb Item " + i.ToString ();
			g.transform.localEulerAngles = new Vector3 (0, 0, i * step);
			g.transform.GetChild (1).GetComponent<Text> ().text = "Herb Item  " + i.ToString ();
			g.transform.GetChild (0).GetComponentInChildren<Text> ().text = Random.Range(0,20).ToString();
			allItems.Add (g.transform.GetChild (0));
			if (i >= activeItem) {
				g.GetComponent<CanvasGroup> ().alpha = 0;
			}
		}
		transform.localEulerAngles = new Vector3 (0, 0, -activeItem * step*.5f);
		restAngle = transform.rotation;
		fixRotationStart ();
		fixRotation ();
	}

	void fixRotationStart()
	{
		foreach (var item in allItems) {
			item.transform.localEulerAngles = new Vector3 (0, 0, -item.parent.localEulerAngles.z-transform.localEulerAngles.z);
		}
	}

	void fixRotation()
	{
		foreach (var item in allItems) {
			item.Rotate (0, 0, -rotateSpeed*direction );
		}
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

	IEnumerator RotateWheel()
	{
		while (rotateSpeed>0) {
			rotateSpeed -= Time.deltaTime*inertia;
			transform.Rotate (0, 0, rotateSpeed*direction );
			fixRotation ();
			yield return null;
		} 

		StartCoroutine (fixRot());

	}

	IEnumerator fixRot()
	{

		if (transform.localEulerAngles.z < clampAngle || transform.localEulerAngles.z >= 355 ) {

			float t = 0;

			while (t <= 1f) {
				t += Time.deltaTime*fixSpeed;
				transform.rotation = Quaternion.Lerp (transform.rotation, restAngle, Mathf.SmoothStep (0, 1f, t));
				fixRotationStart ();
				yield return null;
			}
		}
	}

	public void OnClick(string text)
	{
		StopAllCoroutines ();
		CanRotate = true;
	}

	public void OnRelease()
	{
		CanRotate = false;
		StartCoroutine (RotateWheel());
	}
}
