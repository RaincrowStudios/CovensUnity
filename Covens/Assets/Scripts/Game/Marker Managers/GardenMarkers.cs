using UnityEngine;
using System.Collections;

public class GardenMarkers : MonoBehaviour
{
	public static GardenMarkers Instance{ get; set;}
	public GameObject gardenPrefab;
	public float scale = 10;

	void Awake()
	{
		Instance = this;
	}

	public void CreateGardens()
	{
		foreach (var item in PlayerDataManager.config.gardens) {
			var pos = new Vector2 (item.longitude, item.latitude);  
			OnlineMapsMarker3D marker = new OnlineMapsMarker3D();
			marker = OnlineMapsControlBase3D.instance.AddMarker3D (pos, gardenPrefab);
			marker.scale = scale;
			marker.range = new OnlineMapsRange (3, 12);
		}
	}
}

