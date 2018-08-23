using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof( SwipeDetector))]
public class FlipManager : MonoBehaviour {

	SwipeDetector SD;
	public Transform dailyWheel;
	public Transform playerFeed;
	public float speed = 1;
	CanvasGroup wheelCG;
	CanvasGroup feedCG;
	bool isPlayerFeed = true;
	// Use this for initialization
	void Start () {
		SD = GetComponent<SwipeDetector> ();
		wheelCG = dailyWheel.GetComponent<CanvasGroup> ();
		feedCG = playerFeed.GetComponent<CanvasGroup> ();
		SD.SwipeRight = OnSwipe;
		SD.SwipeLeft = OnSwipe;
	}
	
	void OnSwipe()
	{
		if (isPlayerFeed) {
			StartCoroutine (FlipFeed ());
		} else {
			StartCoroutine (FlipWheel ());
		}
		isPlayerFeed = !isPlayerFeed;
	}

	IEnumerator FlipWheel()
	{
		float t = 0;
		while (t<=0) {
			t += Time.deltaTime * speed;
			FlipHelper (t);
			yield return 0;
		}
	}

	IEnumerator FlipFeed()
	{
		float t = 1;
		while (t>=0) {
			t -= Time.deltaTime * speed;
			yield return 0;
		}
	}

	void FlipHelper(float t)
	{
		playerFeed.rotation = Quaternion.Lerp (Quaternion.Euler (0, 0, 0), Quaternion.Euler (0, 180, 0),  Mathf.SmoothStep (0, 1, t));
		dailyWheel.rotation = Quaternion.Lerp (Quaternion.Euler (180,0, 0), Quaternion.Euler (0, 0, 0),  Mathf.SmoothStep (0, 1, t));
		wheelCG.alpha = Mathf.SmoothStep (0, 1, t);
		feedCG.alpha = Mathf.SmoothStep (1, 0, t);
	}
}
