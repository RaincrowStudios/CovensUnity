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
				var type  = (MarkerSpawner.MarkerType)Enum.Parse (typeof(MarkerSpawner.MarkerType), data.type);

				var it = new InventoryItems();
				it.count = data.count;
				it.displayName = data.displayName;
				it.id = data.id;
//				it.description = data.description;

				if(type == MarkerSpawner.MarkerType.gem){
					if(PlayerDataManager.playerData.ingredients.gemsDict.ContainsKey(it.id)){
						PlayerDataManager.playerData.ingredients.gemsDict[it.id].count += it.count;
					} else{
						PlayerDataManager.playerData.ingredients.gemsDict.Add(it.id,it);
					}
				}
				if(type == MarkerSpawner.MarkerType.tool){
					if(PlayerDataManager.playerData.ingredients.toolsDict.ContainsKey(it.id)){
						PlayerDataManager.playerData.ingredients.toolsDict[it.id].count += it.count;
					} else{
						PlayerDataManager.playerData.ingredients.toolsDict.Add(it.id,it);
					}
				}
				if(type == MarkerSpawner.MarkerType.herb){
					if(PlayerDataManager.playerData.ingredients.herbsDict.ContainsKey(it.id)){
						PlayerDataManager.playerData.ingredients.herbsDict[it.id].count += it.count;
					} else{
						PlayerDataManager.playerData.ingredients.herbsDict.Add(it.id,it);
					}
				}
				data.degree = UnityEngine.Random.Range(-2,3);
				InventoryPickUpManager.Instance.OnCollectSuccess(data);
				MarkerSpawner.SelectedMarker = data;
			}catch(Exception e) {
				Debug.LogError (e);
			}
		}
	}
}

