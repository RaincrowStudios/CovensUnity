using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerManager : MonoBehaviour {
	
	public static Dictionary<string, List<OnlineMapsMarker3D>> Markers = new Dictionary<string, List<OnlineMapsMarker3D>>();
	OnlineMapsControlBase3D control;

	void Start()
	{
		control = OnlineMapsControlBase3D.instance;
	}

	public void DeleteAllMarkers( )
	{
		foreach (var item in Markers) {
			foreach (var marker in item.Value) {
				try{
					
					marker.control.RemoveMarker3D(marker);
				} catch(System.Exception e) {
					var s = marker.customData as MarkerData;
					print(s.type);
					Debug.LogError (e.ToString());
				}
			}
		}
		Markers.Clear ();
	}

	public void DeleteMarker(string ID)
	{
		if (Markers.ContainsKey (ID)) {
			foreach (var marker in Markers[ID]) {
				OnlineMapsControlBase3D.instance.RemoveMarker3D (marker);
			}
		}
		Markers.Remove (ID);
	}
}

