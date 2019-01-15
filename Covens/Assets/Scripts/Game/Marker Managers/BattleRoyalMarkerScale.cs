using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRoyalMarkerScale : MonoBehaviour {

	public int zoom11 = 10;
	public int zoom12 = 15;
	public int zoom13 = 20;
	public int zoom14 = 25;
	public int zoom15 = 30;
	public int zoom16 = 35;
	public int zoom17 = 40;
	public int zoom18 = 45;

	// Use this for initialization
	void OnEnable() {
		zoom ();
		MapsAPI.Instance.OnChangeZoom += zoom;
	}

	void OnDisable()
	{
		MapsAPI.Instance.OnChangeZoom -= zoom;
	}

	void zoom()
	{
        Raincrow.Maps.IMaps map = MapsAPI.Instance;
		float y = 35;

		if (map.zoom == 11)
			y = zoom11;
		if (map.zoom == 12)
			y = zoom12;

		if (map.zoom == 13)
			y = zoom13;

		if (map.zoom == 14)
			y = zoom14;

		if (map.zoom == 15)
			y =zoom15;

		if (map.zoom == 16)
			y= zoom16;

		if (map.zoom == 17)
			y = zoom17;

		if (map.zoom == 18)
			y = zoom18;
		
		transform.localScale = new Vector3 (y,y,y);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
