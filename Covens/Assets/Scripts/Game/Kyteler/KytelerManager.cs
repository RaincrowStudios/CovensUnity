using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

public class KytelerData
{
    public string id;
    public string title;
    public string description;
    public string iconId;
    public string artId;
}

public class KytelerManager : MonoBehaviour
{
    public static void GetKnownRings(Action<int, List<KytelerItem>> callback)
    {
        //APIManager.Instance.GetData("character/kyteler", (string s, int r) =>
        //{
        //    Debug.Log(s);

        //    if (r == 200)
        //    {
        //        callback(r, JsonConvert.DeserializeObject<List<KytelerItems>>(s));
        //    }
        //    else
        //    {
        //        callback(r, null);
        //    }
        //});

        List<KytelerItem> list = new List<KytelerItem>()
        {
            new KytelerItem() {
                id = "ring-3",
                location = "",
                discoveredOn = 0,
                ownerName = "Bashelik"
            },
            new KytelerItem() {
                id = "ring-1",
                location = "",
                discoveredOn = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds,
                ownerName = PlayerDataManager.playerData.displayName
            },
        };

        callback(200, list);
    }

    public static List<KytelerData> GetAllRings()
    {
        //TODO: load from downloaded assets;

        List<KytelerData> list = new List<KytelerData>()
        {
            new KytelerData() {
                id = "ring-1",
                title = "ring-1",
                iconId = "ring-1-icon",
                artId = "ring-1-art"
            },
            new KytelerData() {
                id = "ring-2",
                title = "ring-2",
                iconId = "ring-2-icon",
                artId = "ring-2-art"
            },
            new KytelerData() {
                id = "ring-3",
                title = "ring-3",
                iconId = "ring-3-icon",
                artId = "ring-3-art"
            },
            new KytelerData() {
                id = "ring-4",
                title = "ring-4",
                iconId = "ring-4-icon",
                artId = "ring-4-art"
            },
            new KytelerData() {
                id = "ring-5",
                title = "ring-5",
                iconId = "ring-5-icon",
                artId = "ring-5-art"
            },
            new KytelerData() {
                id = "ring-6",
                title = "ring-6",
                iconId = "ring-6-icon",
                artId = "ring-6-art"
            },
            new KytelerData() {
                id = "ring-7",
                title = "ring-7",
                iconId = "ring-7-icon",
                artId = "ring-7-art"
            },
            new KytelerData() {
                id = "ring-8",
                title = "ring-8",
                iconId = "ring-8-icon",
                artId = "ring-8-art"
            },
        };

        return list;
    }
}
