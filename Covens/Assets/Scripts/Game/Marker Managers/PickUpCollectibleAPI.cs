using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json;

public static class PickUpCollectibleAPI
{

    public static void pickUp(string instanceID, System.Action<MarkerDataDetail> callback)
    {
        var data = new MapAPI();
        data.target = instanceID;
        APIManager.Instance.PostData(
            "map/pickup",
            JsonConvert.SerializeObject(data),
            (s, i) => SendResetCodeCallback(s, i, instanceID, callback)
        );
    }

    static void SendResetCodeCallback(string result, int response, string instance, System.Action<MarkerDataDetail> callback)
    {
        if (response == 200)
        {
            Debug.Log(result);
            try
            {
                var data = JsonConvert.DeserializeObject<MarkerDataDetail>(result);
                var type = (MarkerSpawner.MarkerType)Enum.Parse(typeof(MarkerSpawner.MarkerType), data.type);

                var it = new InventoryItems();
                it.count = data.count;
                it.displayName = data.displayName;
                it.id = data.id;
                it.rarity = DownloadedAssets.ingredientDictData[it.id].rarity;
                it.name = DownloadedAssets.ingredientDictData[it.id].name;

                if (type == MarkerSpawner.MarkerType.gem)
                {
                    if (PlayerDataManager.playerData.ingredients.gemsDict.ContainsKey(it.id))
                        Debug.Log(PlayerDataManager.playerData.ingredients.gemsDict[it.id].count);
                    if (PlayerDataManager.playerData.ingredients.gemsDict.ContainsKey(it.id))
                    {
                        PlayerDataManager.playerData.ingredients.gemsDict[it.id].count += it.count;
                    }
                    else
                    {
                        PlayerDataManager.playerData.ingredients.gemsDict.Add(it.id, it);
                        PlayerDataManager.playerData.ingredients.gems.Add(it);
                    }
                    Debug.Log(PlayerDataManager.playerData.ingredients.gemsDict[it.id].count);
                }
                if (type == MarkerSpawner.MarkerType.tool)
                {
                    if (PlayerDataManager.playerData.ingredients.toolsDict.ContainsKey(it.id))
                        Debug.Log(PlayerDataManager.playerData.ingredients.toolsDict[it.id].count);

                    if (PlayerDataManager.playerData.ingredients.toolsDict.ContainsKey(it.id))
                    {
                        PlayerDataManager.playerData.ingredients.toolsDict[it.id].count += it.count;
                    }
                    else
                    {
                        PlayerDataManager.playerData.ingredients.toolsDict.Add(it.id, it);
                        PlayerDataManager.playerData.ingredients.tools.Add(it);
                    }
                    Debug.Log(PlayerDataManager.playerData.ingredients.toolsDict[it.id].count);

                }
                if (type == MarkerSpawner.MarkerType.herb)
                {
                    if (PlayerDataManager.playerData.ingredients.herbsDict.ContainsKey(it.id))
                        Debug.Log(PlayerDataManager.playerData.ingredients.herbsDict[it.id].count);

                    if (PlayerDataManager.playerData.ingredients.herbsDict.ContainsKey(it.id))
                    {
                        PlayerDataManager.playerData.ingredients.herbsDict[it.id].count += it.count;
                    }
                    else
                    {
                        PlayerDataManager.playerData.ingredients.herbsDict.Add(it.id, it);
                        PlayerDataManager.playerData.ingredients.herbs.Add(it);
                    }
                    Debug.Log(PlayerDataManager.playerData.ingredients.herbsDict[it.id].count);

                }


                callback?.Invoke(data);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                callback?.Invoke(null);
            }
        }
        else
        {
            callback?.Invoke(null);
        }
    }
}

