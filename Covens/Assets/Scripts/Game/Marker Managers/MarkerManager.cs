using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerManager : MonoBehaviour {
	
	public static Dictionary<string, List<OnlineMapsMarker3D>> Markers = new Dictionary<string, List<OnlineMapsMarker3D>>();
	public static Dictionary<string,bool> StanceDict = new Dictionary<string,bool> ();
	OnlineMapsControlBase3D control;

	void Start()
	{
		control = OnlineMapsControlBase3D.instance;
	}

	public static void DeleteAllMarkers( )
	{
		foreach (var item in Markers) {
			foreach (var marker in item.Value) {
				try{
					marker.control.RemoveMarker3D(marker);
				} catch(System.Exception e) {
					var s = marker.customData as Token;
					print(s.type);
					Debug.LogError (e.ToString());
				}
			}
		}
		MarkerSpawner.ImmunityMap.Clear ();
		Markers.Clear ();
	}

	public static void DeleteMarker(string ID)
	{
		print ("Trying to remove : " + ID);
		if (Markers.ContainsKey (ID)) {
			foreach (var marker in Markers[ID]) {
				marker.control.RemoveMarker3D (marker);
			}
		}
		if (MarkerSpawner.ImmunityMap.ContainsKey (ID))
			MarkerSpawner.ImmunityMap.Remove (ID);
		Markers.Remove (ID);
	}

	public static void SetImmunity(bool isImmune,string id)
	{
		if (isImmune) {
			if (Markers.ContainsKey (id)) {
				Markers [id] [0].instance.GetComponentInChildren<SpriteRenderer> ().color = new Color (1, 1, 1, .3f);
			}
		} else {
			if (Markers.ContainsKey (id)) {
				Markers [id] [0].instance.GetComponentInChildren<SpriteRenderer> ().color = Color.white;
			}
		}
	}
}

