using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRoyalMarker : MonoBehaviour {
	public GameObject Marker;
	public float MarkerScale = 10;
	public Vector2 Coordinate;
	// Use this for initialization
	void Awake () {

	}
	
	// Update is called once per frame
	void Start () {
		var pos = new Vector2 (Coordinate.y,Coordinate.x);
		var marker =  OnlineMapsControlBase3D.instance.AddMarker3D (pos, Marker);
		marker.scale = MarkerScale;
		marker.range = new OnlineMapsRange (11, 20);
	}
}
