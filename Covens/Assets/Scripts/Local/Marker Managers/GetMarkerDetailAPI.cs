using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json;

public class GetMarkerDetailAPI : MonoBehaviour
{
	static string charName;
	public static void GetData(string characterName, MarkerSpawner.MarkerType type)
	{
		charName = characterName;
		var data = new MapAPI ();
		data.type = type.ToString ();
		data.target = characterName;
		Action<string,int, MarkerSpawner.MarkerType> callback;
		callback = SendResetCodeCallback;
		APIManager.Instance.PostCovenSelect ("map/select", JsonConvert.SerializeObject(data), callback,type);
	}

	static void SendResetCodeCallback (string result, int response, MarkerSpawner.MarkerType type)
	{
		if (response == 200) {
			print (result);
			try{
				var data = JsonConvert.DeserializeObject<MarkerDetailContainer> (result);
				data.selection.displayName = charName;
				data.selection.alignment = UnityEngine.Random.Range(-2,3);
				MarkerSpawner.SelectedMarker = data.selection;
				if(type == MarkerSpawner.MarkerType.witch){
					EventManager.Instance.CallPlayerDataReceivedEvent();
				}
			}catch(Exception e) {
				print (e);
			}
		}
	}
}

