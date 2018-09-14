using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class StoreManagerAPI : MonoBehaviour {

	public static void GetShopItems(Action<string,int> data)
	{
		APIManager.Instance.PostData ("shop/display", "GiMMeAllzEShitzz", data, true);
	}

	public static void PurchaseItem(string itemID, Action<string,int>data){
		var js = new {purchase = itemID}; 
		APIManager.Instance.PostData ("shop/purchase", JsonConvert.SerializeObject (js),data);
	}
}


public class StoreItemContent
{
	public string id { get; set; }
	public int count { get; set; }
}

public class StoreApiItem
{
	public string id { get; set; }
	public string title { get; set; }
	public string type { get; set; }
	public int amount { get; set; }
	public string bonus { get; set; }
	public float cost { get; set; }
	public int silver { get; set; }
	public int gold { get; set; }
	public List<StoreItemContent> contents { get; set; }
	public bool owned { get; set; }
	[NonSerialized]
	public Sprite pic;
}

