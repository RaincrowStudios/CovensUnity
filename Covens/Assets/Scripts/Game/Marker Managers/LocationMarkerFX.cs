using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationMarkerFX : MonoBehaviour {
	OnlineMaps map;
	public GameObject g;
	// Use this for initialization
	void OnEnable()
	{
		map = OnlineMaps.instance;
		map.OnChangeZoom += SetGlyph;
	}

	void OnDisable()
	{
		map.OnChangeZoom -= SetGlyph;

	}

	void OnDestroy()
	{
		map.OnChangeZoom -= SetGlyph;
	}

	void SetGlyph()
	{
		if (map.zoom > 15) {
			g.SetActive (true);
		} else
			g.SetActive (false);
	}
}
