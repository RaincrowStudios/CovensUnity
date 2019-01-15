using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Maps;

public class MarkerScaleManager : MonoBehaviour {
	 public float iniScale;
	 public float s;
    public IMarker m;


	void OnEnable()
	{
        MapsAPI.Instance.OnMapUpdated += fixScale;
		EventManager.OnSmoothZoom += fixScale;
		EventManager.OnFreezeScale += manageFreezeZoom;
	}

	void manageFreezeZoom(bool scale)
	{
		if (scale) {
            MapsAPI.Instance.OnMapUpdated += fixScale;
            EventManager.OnSmoothZoom += fixScale;

		} else {
            MapsAPI.Instance.OnMapUpdated -= fixScale;
            EventManager.OnSmoothZoom -= fixScale;
		}
	}

	void OnDestroy()
	{
		EventManager.OnSmoothZoom -= fixScale;
        MapsAPI.Instance.OnMapUpdated -= fixScale;
	}

	public void fixScale()
	{
        s = MapsAPI.Instance.transform.localScale.x;
		m.scale = iniScale / s;
	}

    [ContextMenu("print m")]
    private void _PrintMarker()
    {
        Debug.Log(m.instance);
    }
}
