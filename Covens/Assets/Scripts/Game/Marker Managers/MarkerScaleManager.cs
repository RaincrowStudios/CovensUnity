using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerScaleManager : MonoBehaviour {
	 public float iniScale;
	 public float s;
	OnlineMaps map;
	[HideInInspector]
	public OnlineMapsMarkerBase m;


	void OnEnable()
	{
		map = OnlineMaps.instance;
		map.OnMapUpdated += fixScale;
		EventManager.OnSmoothZoom += fixScale;
		EventManager.OnFreezeScale += manageFreezeZoom;
		Invoke ("getStuff", .03f);
		fixScale ();
	}

	void manageFreezeZoom(bool scale)
	{
		if (scale) {
			map.OnMapUpdated += fixScale;
			EventManager.OnSmoothZoom += fixScale;
		} else {
			map.OnMapUpdated -= fixScale;
			EventManager.OnSmoothZoom -= fixScale;
		}
	}

	void getStuff()
	{
		m = GetComponent<OnlineMapsMarker3DInstance> ().marker;
	}

	void OnDestroy()
	{
		EventManager.OnSmoothZoom -= fixScale;
		map.OnMapUpdated -= fixScale;

	}

	public void fixScale()
	{
		s = map.transform.localScale.x; 
		m.scale = iniScale / s;
	}

}
