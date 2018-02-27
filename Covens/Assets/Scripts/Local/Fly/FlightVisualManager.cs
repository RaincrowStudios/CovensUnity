using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FlightVisualManager : MonoBehaviour {

	float t;
	public float speed = 1;
	public CanvasGroup[] groups;

	bool fly;
	bool land;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (fly) {
			if (t < 1) {
				t += Time.deltaTime * speed;
				foreach (var item in groups) {
					item.alpha = Mathf.Lerp (1, .2f, t);
				}
			} else {
				fly = false;
			}
		}

		if (land) {
			if (t < 1) {
				t += Time.deltaTime * speed;
				foreach (var item in groups) {
					item.alpha = Mathf.Lerp (.2f, 1f, t);
				}
			} else {
				fly = false;
			}
		}
	}

	public void FadeIn()
	{
		land = true;
		fly = false;
		t = 0;
		foreach (var item in groups) {
			item.blocksRaycasts = true;
		}
	}

	public void FadeOut()
	{
		land = false;
		fly = true;
		t = 0;
		foreach (var item in groups) {
			item.blocksRaycasts = false;
		}
	}

}
