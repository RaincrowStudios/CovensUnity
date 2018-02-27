using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerScaleManager : MonoBehaviour {
	public float iniScale;
	public float s;
	float curZoom,prevZoom =0;
	OnlineMaps map;

	public OnlineMapsMarker3D m;


	void OnEnable()
	{
		map = OnlineMaps.instance;
		EventManager.OnSmoothZoom += fixScale;
		fixScale ();
	}

	void OnDestroy()
	{
		EventManager.OnSmoothZoom -= fixScale;

	}

	public void fixScale()
	{
		s = map.transform.localScale.x; 
		m.scale = iniScale / s;
//		curZoom = map.zoom;
//		if (curZoom != prevZoom) {
//			m.scale = iniScale;
//		}
//		prevZoom = curZoom;
	}

}
