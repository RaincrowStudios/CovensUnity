using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[RequireComponent (typeof(MarkerSpawner))]
public class MarkerManagerAPI : MonoBehaviour
{
	public static void GetMarkers (bool isPhysical = true)
	{
		var data = new MapAPI ();
		data.characterName = PlayerDataManager.playerData.displayName; 
		data.physical = isPhysical; 
		if (isPhysical) {
			data.longitude = PlayerDataManager.playerPos.x;
			data.latitude = PlayerDataManager.playerPos.y;
		} else {
			data.longitude = OnlineMaps.instance.position.x;
			data.latitude = OnlineMaps.instance.position.y;
		}
		Action<string,int> callback;
		callback = GetMarkersCallback;
		APIManager.Instance.PostCoven ("map/move", JsonConvert.SerializeObject (data), callback);
	}

	static void GetMarkersCallback (string result, int response)
	{
		if (response == 200) {
			try {
				print(result);
				var data = JsonConvert.DeserializeObject<MarkerAPI> (result);

				if(OnlineMapsUtils.DistanceBetweenPointsD(new Vector2((float) data.location.longitude, (float) data.location.latitude),PlayerManager.marker.position)>1){
					OnlineMaps.instance.SetPosition(data.location.longitude,data.location.latitude);
					PlayerManager.marker.position = OnlineMaps.instance.position;
				}
				MarkerSpawner.Instance.CreateMarkers ( AddEnumValue (data.tokens));   
			} catch (Exception e) {
				Debug.LogError (e.ToString ());
			}
		}
	}

	public static List<Token> AddEnumValue (List<Token> data)
	{
		var updatedData = new List<Token> ();
		foreach (Token item in data) {
			try{
				item.Type = (MarkerSpawner.MarkerType)Enum.Parse (typeof(MarkerSpawner.MarkerType), item.type);
			updatedData.Add (item);
			}catch{
			}
		}
		return updatedData;
	}

	public static Token AddEnumValueSingle (Token data)
	{
		data.Type = (MarkerSpawner.MarkerType)Enum.Parse (typeof(MarkerSpawner.MarkerType), data.type);
		return data;
	}
}

