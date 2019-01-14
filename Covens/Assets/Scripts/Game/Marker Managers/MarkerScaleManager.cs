using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Maps;

public class MarkerScaleManager : MonoBehaviour {
	 public float iniScale;
	 public float s;
    //OnlineMaps map;
    //[HideInInspector]
    //public OnlineMapsMarker3D m;
    public IMarker m;


	void OnEnable()
	{
        //map = OnlineMaps.instance;
        //map.OnMapUpdated += fixScale;
        MapsAPI.Instance.OnMapUpdated += fixScale;
		EventManager.OnSmoothZoom += fixScale;
		EventManager.OnFreezeScale += manageFreezeZoom;
	}

	void manageFreezeZoom(bool scale)
	{
		if (scale) {
            //map.OnMapUpdated += fixScale;
            MapsAPI.Instance.OnMapUpdated += fixScale;
            EventManager.OnSmoothZoom += fixScale;

		} else {
            //map.OnMapUpdated -= fixScale;
            MapsAPI.Instance.OnMapUpdated -= fixScale;
            EventManager.OnSmoothZoom -= fixScale;
		}
	}

	void OnDestroy()
	{
		EventManager.OnSmoothZoom -= fixScale;
        MapsAPI.Instance.OnMapUpdated -= fixScale;
        //map.OnMapUpdated -= fixScale;

	}

	public void fixScale()
	{
        //s = map.transform.localScale.x; 
        s = MapsAPI.Instance.transform.localScale.x;
		m.scale = iniScale / s;
	}

}
