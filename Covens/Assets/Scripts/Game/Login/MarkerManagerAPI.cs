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
		APIManager.Instance.PostCoven ("map/move", JsonConvert.SerializeObject (data), callback);
	}

	static void GetMarkersCallback (string result, int response)
	{
		if (response == 200) {
			try {
				var data = JsonConvert.DeserializeObject<MarkerAPI> (result);
				MarkerSpawner.Instance.CreateMarkers (AddEnumValue (data.tokens));   
			} catch (Exception e) {
				print (e.ToString ());
			}
		}
	}

	public static List<Token> AddEnumValue (List<Token> data)
	{
		var updatedData = new List<Token> ();
		foreach (Token item in data) {
			try{
			if (item.type == "portal") {
				if (item.subtype == "lesser")
					item.Type = MarkerSpawner.MarkerType.lesserPortal;
				else if (item.subtype == "greater")
					item.Type = MarkerSpawner.MarkerType.greaterPortal;
			} else if (item.type == "spirit") {
				if (item.subtype == "lesser")
					item.Type = MarkerSpawner.MarkerType.lesserSpirit;
				else if (item.subtype == "greater")
					item.Type = MarkerSpawner.MarkerType.greaterSpirit;
			} else {
				item.Type = (MarkerSpawner.MarkerType)Enum.Parse (typeof(MarkerSpawner.MarkerType), item.type);
			}
			updatedData.Add (item);
			}catch{
			}
		}
		return updatedData;
	}

	public static Token AddEnumValueSingle (Token data)
	{
		{
			if (data.type == "portal") { 
				if (data.subtype == "lesser")
					data.Type = MarkerSpawner.MarkerType.lesserPortal;
				else if (data.subtype == "greater")
					data.Type = MarkerSpawner.MarkerType.greaterPortal;
			} else if (data.type == "spirit") {
				if (data.subtype == "lesser")
					data.Type = MarkerSpawner.MarkerType.lesserSpirit;
				else if (data.subtype == "greater")
					data.Type = MarkerSpawner.MarkerType.greaterSpirit;
			} else {
				data.Type = (MarkerSpawner.MarkerType)Enum.Parse (typeof(MarkerSpawner.MarkerType), data.type);
			}
			return data;
		}
	}
}

