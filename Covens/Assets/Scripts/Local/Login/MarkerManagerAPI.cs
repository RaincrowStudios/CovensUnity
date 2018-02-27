using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
[RequireComponent(typeof(MarkerSpawner))]
public class MarkerManagerAPI : MonoBehaviour
{
	public static void GetMarkers(bool isPhysical = true)
	{
		var data = new MapAPI ();
		data.characterName = PlayerDataManager.playerData.displayName; 
		data.physical = true; 
		if (isPhysical) {
			data.longitude = OnlineMapsLocationService.instance.position.x;
			data.latitude = OnlineMapsLocationService.instance.position.y;
		} else {
			data.longitude = OnlineMaps.instance.position.x;
			data.latitude = OnlineMaps.instance.position.y;
		}
		Action<string,int> callback;
		callback = GetMarkersCallback;
		APIManager.Instance.PostCoven ("map/location", JsonConvert.SerializeObject (data), callback);
	}

	static void GetMarkersCallback (string result, int response)
	{
		if (response == 200) {
			print (result);
			try{
			var data = JsonConvert.DeserializeObject<MarkerAPI> (result);
				MarkerSpawner.Instance.CreateMarkers (AddEnumValue(data.tokens));   
			}catch(Exception e) {
				print (e.ToString());
			}
		}
	}

	static List<MarkerData> AddEnumValue (List<MarkerData> data)  
	{
		var updatedData = new List<MarkerData> ();
		foreach (MarkerData item in data) {
			item.token.Type = (MarkerSpawner.MarkerType)Enum.Parse (typeof(MarkerSpawner.MarkerType), item.type);
			updatedData.Add (item);
		}
		return updatedData;
	}
}

