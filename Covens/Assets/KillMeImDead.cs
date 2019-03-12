using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillMeImDead : MonoBehaviour {

	public GameObject greyHandOfficFab;

	void Start(){
		InventoryItems item = new InventoryItems();
		item.id = "coll_dreamcatcher";
		item.name = DownloadedAssets.ingredientDictData[item.id].name;
		item.count = 4;
		item.rarity = DownloadedAssets.ingredientDictData[item.id].rarity;
		item.forbidden = true;
		PlayerDataManager.playerData.ingredients.tools.Add (item);
		PlayerDataManager.playerData.ingredients.toolsDict[item.id] = item;
	}


	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.F)) {
			Instantiate (greyHandOfficFab, transform);
		}
	}
}
