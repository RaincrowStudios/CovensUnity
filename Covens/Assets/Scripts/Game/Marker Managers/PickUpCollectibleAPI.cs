using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json;

public static class PickUpCollectibleAPI
{
    public class PickUpResult
    {
        public string id;
        public string type;
        public int count;
        public int xpGain;
    }


    public static void PickUpCollectable(string instance, string type)
    {
        PickUpCollectibleAPI.pickUp(instance, res =>
        {
            if (res == null)
            {
                string msg = "Failed to collect the item.";
                PlayerNotificationManager.Instance.ShowNotification(msg, UICollectableInfo.Instance.m_IconDict[type]);
            }
            else
            {
                IngredientData ingr = DownloadedAssets.GetCollectable(res.id);
                Ingredients ings = PlayerDataManager.playerData.ingredients;
                //string msg = "Added " + res.count.ToString() + " " + (ingr == null ? "ingredient" : ingr.name) + " to the inventory";
                string msg = "<b>+" + res.count.ToString() + "</b> <color=#FFAE00>" + LocalizeLookUp.GetCollectableName(res.id) + "</color> collected. Current Total: <b>";
                if (ingr.type == "tool")
                {
                    Debug.Log("it's tool");
                    msg += ings.toolsDict[res.id].count.ToString();
                }
                else if (ingr.type == "gem")
                {
                    Debug.Log("it's gem");
                    msg += ings.gemsDict[res.id].count.ToString();
                }
                else if (ingr.type == "herb")
                {
                    Debug.Log("it's herb");
                    msg += ings.herbsDict[res.id].count.ToString();
                }
                else
                {
                    Debug.Log("you got something wrong");
                }
                msg += "</b>";
                PlayerNotificationManager.Instance.ShowNotification(msg, UICollectableInfo.Instance.m_IconDict[type]);
                SoundManagerOneShot.Instance.PlayItemAdded();
            }
        });
    }

    private static void pickUp(string instanceID, System.Action<PickUpResult> callback)
    {
        var data = new MapAPI();
        data.target = instanceID;
        APIManager.Instance.Post(
            "map/pickup",
            JsonConvert.SerializeObject(data),
            (s, i) => SendResetCodeCallback(s, i, instanceID, callback)
        );
    }

    private static void SendResetCodeCallback(string result, int response, string instance, System.Action<PickUpResult> callback)
    {
        if (response == 200)
        {
            Debug.Log(result);
            try
            {
                var data = JsonConvert.DeserializeObject<PickUpResult>(result);
                var type = (MarkerSpawner.MarkerType)Enum.Parse(typeof(MarkerSpawner.MarkerType), data.type);

                var it = new CollectableItem();
                it.count = data.count;
                it.collectible = data.id;

                if (type == MarkerSpawner.MarkerType.gem)
                {
                    if (PlayerDataManager.playerData.ingredients.gemsDict.ContainsKey(it.collectible))
                        Debug.Log(PlayerDataManager.playerData.ingredients.gemsDict[it.collectible].count);
                    if (PlayerDataManager.playerData.ingredients.gemsDict.ContainsKey(it.collectible))
                    {
                        PlayerDataManager.playerData.ingredients.gemsDict[it.collectible].count += it.count;
                    }
                    else
                    {
                        PlayerDataManager.playerData.ingredients.gemsDict.Add(it.collectible, it);
                    }
                    Debug.Log(PlayerDataManager.playerData.ingredients.gemsDict[it.collectible].count);
                }
                if (type == MarkerSpawner.MarkerType.tool)
                {
                    if (PlayerDataManager.playerData.ingredients.toolsDict.ContainsKey(it.collectible))
                        Debug.Log(PlayerDataManager.playerData.ingredients.toolsDict[it.collectible].count);

                    if (PlayerDataManager.playerData.ingredients.toolsDict.ContainsKey(it.collectible))
                    {
                        PlayerDataManager.playerData.ingredients.toolsDict[it.collectible].count += it.count;
                    }
                    else
                    {
                        PlayerDataManager.playerData.ingredients.toolsDict.Add(it.collectible, it);
                    }
                    Debug.Log(PlayerDataManager.playerData.ingredients.toolsDict[it.collectible].count);

                }
                if (type == MarkerSpawner.MarkerType.herb)
                {
                    if (PlayerDataManager.playerData.ingredients.herbsDict.ContainsKey(it.collectible))
                        Debug.Log(PlayerDataManager.playerData.ingredients.herbsDict[it.collectible].count);

                    if (PlayerDataManager.playerData.ingredients.herbsDict.ContainsKey(it.collectible))
                    {
                        PlayerDataManager.playerData.ingredients.herbsDict[it.collectible].count += it.count;
                    }
                    else
                    {
                        PlayerDataManager.playerData.ingredients.herbsDict.Add(it.collectible, it);
                    }
                    Debug.Log(PlayerDataManager.playerData.ingredients.herbsDict[it.collectible].count);

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

