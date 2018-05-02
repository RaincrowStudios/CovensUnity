using UnityEngine;
using System.Collections;

public class ZoomFXManager : MonoBehaviour
{
	float scaleVal = 1;
	OnlineMaps map;
	public GameObject Particles;
	public GameObject Particles2;
	// Use this for initialization
	void Start ()
	{
		map = OnlineMaps.instance;
	}
	
	void Update()
	{
		if (Input.touchCount == 2)
		{
			var touch = Input.GetTouch (1);

			if (scaleVal != map.transform.localScale.x) {
				EventManager.Instance.CallSmoothZoom ();
				scaleVal = map.transform.localScale.x;
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

		//		if (Input.GetMouseButtonUp (0)) {
		//			Invoke ("callzoom", .08f);
		//		}
	}
}

