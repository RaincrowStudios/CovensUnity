using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRoyalMarker : MonoBehaviour
{
	public GameObject Marker;
	public float MarkerScale = 10;
	public Vector2 Coordinate;

	void Start () {
		var pos = new Vector2 (Coordinate.y,Coordinate.x);
		var marker =  MapsAPI.Instance.AddMarker(pos, Marker);
		marker.scale = MarkerScale;
		marker.SetRange(11, 20);
	}
}
