using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CarouselSnap : MonoBehaviour {

	public int totalSpells;
	public float halfLength = 1160;
	float step;
	public float moveSpeed =1;
	float previousX;
	float currentX;
	public bool isActive;
	public bool isUp;
	public List<float> thresholds = new List<float> (); 

	RectTransform RT;
	// Use this for initialization
	void Start () {
		RT = GetComponent<RectTransform> ();
		step = halfLength*2 / totalSpells;
		currentX = previousX = RT.anchoredPosition.x;

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton (0)) {
			isActive = true;
			isUp = true;
			return;
		} 
		
			currentX = RT.anchoredPosition.x;
			int x = Mathf.RoundToInt (currentX);
			int y = Mathf.RoundToInt (previousX);

			if (x != y) {
				isActive = true;
			} else {
				isActive = false;
			}
			previousX = currentX;

			if (!isActive && isUp) {
				StartCoroutine (SmoothMove (currentX, snapTo ()));
			}

	}


	IEnumerator SmoothMove( float cur, float final )
	{
		isUp = false;
		float t = 0;
		while (t <= 1f) {
			print ("Fixing");
			t += Time.deltaTime * moveSpeed;
			RT.anchoredPosition = new Vector2 ( Mathf.SmoothStep (cur, final, t),  RT.anchoredPosition.y); 
			yield return null;
		}
	}

	float snapTo()
	{
		 float closest = thresholds.Aggregate((x,y) => Mathf.Abs(x-currentX) < Mathf.Abs(y-currentX) ? x : y);
		return closest;
	}
}
