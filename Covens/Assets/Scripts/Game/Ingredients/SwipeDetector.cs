using UnityEngine;
using System.Collections;
using System;

public class SwipeDetector : MonoBehaviour
{
	private Vector2 fingerDown;
	private Vector2 fingerUp;
	public bool detectSwipeOnlyAfterRelease = false;
	public Action SwipeDown;
	public Action SwipeUp;
	public Action SwipeLeft;
	public Action SwipeRight;
	public float SWIPE_THRESHOLD = 30f;
	public bool canSwipe = false;

	void Start()
	{
		
	}

	void Update()
	{
		if (!canSwipe)
			return;
		if (Application.isEditor) {
			if (Input.GetMouseButtonDown (0)) {
				fingerUp = Input.mousePosition;
				fingerDown = Input.mousePosition;
			}
			if (Input.GetMouseButton(0)) {
				if (!detectSwipeOnlyAfterRelease) {
					fingerDown = Input.mousePosition;
					checkSwipe ();
				}
			}
			if (Input.GetMouseButtonUp (0)) {
				fingerDown = Input.mousePosition;
				checkSwipe ();
			}
		} else {
		foreach (Touch touch in Input.touches) {
			
				if (touch.phase == TouchPhase.Began) {
					fingerUp = touch.position;
					fingerDown = touch.position;
				}

				//Detects Swipe while finger is still moving
				if (touch.phase == TouchPhase.Moved) {
					if (!detectSwipeOnlyAfterRelease) {
						fingerDown = touch.position;
						checkSwipe ();
					}
				}

				//Detects swipe after finger is released
				if (touch.phase == TouchPhase.Ended) {
					fingerDown = touch.position;
					checkSwipe ();
				}
			}
		}
	}

	void checkSwipe()
	{
		//Check if Vertical swipe
		if (verticalMove() > SWIPE_THRESHOLD && verticalMove() > horizontalValMove())
		{
			if (fingerDown.y - fingerUp.y > 0)//up swipe
			{
				OnSwipeUp();
			}
			else if (fingerDown.y - fingerUp.y < 0)//Down swipe
			{
				OnSwipeDown();
			}
			fingerUp = fingerDown;
		}

		//Check if Horizontal swipe
		else if (horizontalValMove() > SWIPE_THRESHOLD && horizontalValMove() > verticalMove())
		{
			if (fingerDown.x - fingerUp.x > 0)//Right swipe
			{
				OnSwipeRight();
			}
			else if (fingerDown.x - fingerUp.x < 0)//Left swipe
			{
				OnSwipeLeft();
			}
			fingerUp = fingerDown;
		}

		//No Movement at-all
		else
		{
		}
	}

	float verticalMove()
	{
		return Mathf.Abs(fingerDown.y - fingerUp.y);
	}

	float horizontalValMove()
	{
		return Mathf.Abs(fingerDown.x - fingerUp.x);
	}

	void OnSwipeUp()
	{
		if (SwipeUp != null)
			SwipeUp ();
	}

	void OnSwipeDown()
	{
		if (SwipeDown != null)
			SwipeDown ();
	}

	void OnSwipeLeft()
	{
		if (SwipeLeft != null)
			SwipeLeft ();
	}

	void OnSwipeRight()
	{
		if (SwipeRight != null)
			SwipeRight ();
	}
}