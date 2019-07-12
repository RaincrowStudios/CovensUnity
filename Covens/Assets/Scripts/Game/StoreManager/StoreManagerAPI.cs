using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class StoreManagerAPI : MonoBehaviour
{

    public static void GetShopItems(Action<string, int> data)
    {
        APIManager.Instance.Get("shop/display", data);
    }

    //public static void PurchaseItem(string itemID, Action<string,int>data){
    //	var js = new {purchase = itemID}; 
    //	APIManager.Instance.PostData ("shop/purchase", JsonConvert.SerializeObject (js),data);
    //}
}


public class StoreItemContent
{
    public string id { get; set; }
    public int count { get; set; }
}

public class StoreApiObject
{
    public List<StoreApiItem> bundles { get; set; }
    public List<CosmeticData> cosmetics { get; set; }
    public List<CosmeticData> styles { get; set; }
    public List<StoreApiItem> consumables { get; set; }
    public List<StoreApiItem> silver { get; set; }
}

public class StoreApiItem
{
    public string id { get; set; }
    public string productId { get; set; }
    public string title { get; set; }
    public string type { get; set; }
    public int amount { get; set; }
    public string bonus { get; set; }
    public float cost { get; set; }
    public int silver { get; set; }
    public int gold { get; set; }
    public List<StoreItemContent> contents { get; set; }
    public bool owned { get; set; }
    [JsonIgnore]
    public Sprite pic;
    [JsonIgnore]
    public int count;
}

