using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

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

        List<KytelerItem> list = new List<KytelerItem>();

#if UNITY_EDITOR
        list = new List<KytelerItem>()
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
                ownerName = PlayerDataManager.playerData.name
            },
        };
#endif

        callback(200, list);
    }

    public static KytelerData[] GetAllRings()
    {
        //TODO: load from downloaded assets;

        return Resources.LoadAll<KytelerData>("KytelerRings");
    }
}
