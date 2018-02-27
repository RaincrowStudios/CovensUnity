using UnityEngine;
using System.Collections;
using System;

public class GetMarkerDetailAPI : MonoBehaviour
{

	public static void GetData(string characterName, MarkerSpawner.MarkerType type)
	{
		var data = new MapAPI ();
		data.characterName = LoginAPIManager.username; 
		data.target = characterName;
		Action<string,int, MarkerSpawner.MarkerType> callback;
		callback = SendResetCodeCallback;
		APIManager.Instance.PostCovenSelect ("map/select", JsonUtility.ToJson (data), callback,type);
	}

	static void SendResetCodeCallback (string result, int response, MarkerSpawner.MarkerType type)
	{
		if (response == 200) {
			print (result);
			try{
				var data = JsonUtility.FromJson<MarkerDataDetail> (result);

			}catch(Exception e) {
				print (e);
			}
		}
	}
}

