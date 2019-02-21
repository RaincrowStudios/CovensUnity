using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapMarkerScaleManager : MonoBehaviour {

	Camera cam;
	public float minScale =.2f;
	public float maxScale =.6f;
	float minZoom;
	float maxZoom;

	// Use this for initialization
	void Start(){
		cam = Camera.main;
		var sM = SpriteMapsController.instance;
		minZoom = sM.m_MinZoom;
		maxZoom = sM.m_MaxZoom;
		sM.OnMapUpdated += updateMarkerScale;
	}

	// Update is called once per frame
	void updateMarkerScale () {
		float sMultiplier = MapUtils.scale (minScale, maxScale, minZoom, maxZoom, cam.orthographicSize);
		foreach (Transform item in transform) {
			item.localScale = Vector3.one * sMultiplier;
		}
	}
}
