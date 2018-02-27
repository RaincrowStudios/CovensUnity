using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritMovementFX : MonoBehaviour { 

	public float moveSpeed = 2;

	public static SpiritMovementFX Instance { get; set;}

	void Awake()
	{
		Instance = this;
	}

	public void MoveSpirit(MarkerData data)
	{
		print ("Checking for spirit : " + data.instance);
		if (MarkerManager.Markers.ContainsKey (data.instance)) {
			print ("SpiritPresent!");
			if (OnlineMaps.instance.zoom > 12) {
				StartCoroutine (SmoothMove (MarkerManager.Markers [data.instance] [0], new Vector2 (data.token.longitude, data.token.latitude)));
			} else {
				foreach (var item in MarkerManager.Markers[data.instance]) {
					item.position = new Vector2 (data.token.longitude, data.token.longitude);
				}
			}
		} else {
			print ("SpiritAbsent!");
			MarkerSpawner.Instance.AddMarker (data);
		}
	}

	IEnumerator SmoothMove( OnlineMapsMarker3D marker , Vector2 final)
	{
		Vector2 init = marker.position;
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime * moveSpeed;
			marker.position = Vector2.Lerp (init, final, Mathf.SmoothStep (0, 1f, t));
			yield return null;
		}
	}

}
