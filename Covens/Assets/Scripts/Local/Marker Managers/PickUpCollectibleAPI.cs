using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json;

public class PickUpCollectibleAPI : MonoBehaviour
{

	public static void pickUp(string instanceID)
	{
		var data = new MapAPI ();
		data.target = instanceID;
		Action<string,int> callback;
		callback = SendResetCodeCallback;
		APIManager.Instance.PostCoven ("map/pickup", JsonConvert.SerializeObject(data), callback);
	}

	static void SendResetCodeCallback (string result, int response )
	{
		if (response == 200) {
			print (result);
			try{
				var data = JsonConvert.DeserializeObject<MarkerDataDetail> (result);
				PlayerDataManager.playerData.inventory = LoginAPIManager.DictifyData(data).inventory;
				data.degree = UnityEngine.Random.Range(-2,3);
				CollectibleSelect.Instance.OnCollectSuccess(data);
				MarkerSpawner.SelectedMarker = data;
			}catch(Exception e) {
				Debug.LogError (e);
			}
		}
	}
}

