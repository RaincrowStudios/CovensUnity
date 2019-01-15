using UnityEngine;
using System.Collections;

public class ZoomFXManager : MonoBehaviour
{
	float scaleVal = 1;
	public GameObject Particles;
	public GameObject Particles2;
	
	void Update()
	{
		if (Input.touchCount == 2)
		{
			var touch = Input.GetTouch (1);

			if (scaleVal != MapsAPI.Instance.transform.localScale.x) {
				EventManager.Instance.CallSmoothZoom ();
				scaleVal = MapsAPI.Instance.transform.transform.localScale.x;
				Particles.SetActive (true);
				Particles2.SetActive (true);
			}

						if (touch.phase == TouchPhase.Ended) {
				if (Particles.activeInHierarchy) {
					Particles.SetActive (false);
					Particles2.SetActive (false);
				}
						}
			
		}
	}
}

