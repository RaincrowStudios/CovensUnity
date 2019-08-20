using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Raincrow.Store
{
    public struct IngredientBundleData
    {
        public int silver;
        public int gold;
        public string[] collectables;
        public int[] amount;
    }

    public struct ConsumableData
    {
        public int silver;
        public int gold;
        public float duration;
    }

    public struct SilverBundleData
    {
        [DefaultValue("")]
        public string product;
        public float cost;
        public int amount;
        [DefaultValue("")]
        public string extra;
    }

    public struct StoreItem
    {
        [DefaultValue("")]
        public string id;
        public double unlockOn;
        [DefaultValue("")]
        public string tooltip;
        public int silver;
        public int gold;
    }

    public struct StoreData
    {
        public List<StoreItem> Bundles;
        public List<StoreItem> Consumables;
        public List<StoreItem> Silver;
        public List<StoreItem> Cosmetics;
        public List<StoreItem> Styles;
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

        [JsonIgnore]
        public bool owned => PlayerDataManager.playerData.inventory.cosmetics.Exists(item => item.id == id);

        [JsonIgnore]
        public Sprite pic;
        [JsonIgnore]
        public int count;
    }
       
    public static class StoreManagerAPI
    {
        public static StoreData Store { get; set; }

        private static StoreApiObject m_OldStore = null;
        public static StoreApiObject OldStore
        {
            get
            {
                if (m_OldStore == null)
                    SetupOldStore(Store);
                return m_OldStore;
            }
        }

        public static Dictionary<string, IngredientBundleData> BundleDict { get; set; }
        public static Dictionary<string, ConsumableData> ConsumableDict { get; set; }
        public static Dictionary<string, SilverBundleData> SilverBundleDict { get; set; }

        //public static void PurchaseItem(string itemID, Action<string,int>data){
        //	var js = new {purchase = itemID}; 
        //	APIManager.Instance.PostData ("shop/purchase", JsonConvert.SerializeObject (js),data);
        //}

        public static void Purchase(string id, string type, string currency, System.Action<string> callback)
        {
            Purchase(id, type, currency, null, callback);
        }

        public static void Purchase(string id, string type, string currency, string receipt, System.Action<string> callback)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("itemId", id);
            data.Add("itemType", type);
            if (currency != null)
                data.Add("currency", currency);
            if (receipt != null)
                data.Add("receipt", receipt);

            Debug.Log("<color=magenta>" + id + "</color>");

            APIManager.Instance.Post("shop/purchase", JsonConvert.SerializeObject(data), (response, result) =>
            {
                if (result == 200)
                {
                    string debug = "<color=magenta>purchase complete:</color>\n";
                    int silver = 0;
                    int gold = 0;

                    switch (type)
                    {
                        case "silver":
                            debug += "[silver] " + id;
                            //silver is processed on IAPSilver
                            break;

                        case "cosmetics":
                            debug += "[cosmetics] " + id;
                            CosmeticData cosmetic = DownloadedAssets.GetCosmetic(id);

                            //get the price
                            if (currency == "silver")
                                silver = cosmetic.silver;
                            if (currency == "gold")
                                gold = cosmetic.gold;

                            //add the item to inventory
                            PlayerDataManager.playerData.inventory.cosmetics.Add(cosmetic);
                            break;

                        case "bundles":
                            debug += "[bundles] " + id;
                            IngredientBundleData bundle = StoreManagerAPI.GetBundle(id);

                            //get the price
                            if (currency == "silver")
                                silver = bundle.silver;
                            if (currency == "gold")
                                gold = bundle.gold;

                            //add the ingredients to the inventory
                            for (int i = 0; i < bundle.collectables.Length; i++)
                            {
                                debug += "\n\t" + bundle.collectables[i] + " +" + bundle.amount[i];
                                PlayerDataManager.playerData.AddIngredient(bundle.collectables[i], bundle.amount[i]);
                            }
                            break;

                        case "consumables":
                            debug += "[consumables] " + id;

                            Item item = PlayerDataManager.playerData.inventory.consumables.Find(it => it.id == id);
                            ConsumableData consumable = StoreManagerAPI.GetConsumable(id); ;

                            //get the price
                            if (currency == "silver")
                                silver = consumable.silver;
                            if (currency == "gold")
                                gold = consumable.gold;

                            //add to the inventory
                            if (item == null || string.IsNullOrEmpty(item.id))
                            {
                                PlayerDataManager.playerData.inventory.consumables.Add(new Item
                                {
                                    id = id,
                                    count = 1
                                });
                            }
                            else
                            {
                                item.count += 1;
                            }
                            break;
                    }

                    if (silver != 0)
                    {
                        debug += "\nlost " + silver + " drachs";
                        PlayerDataManager.playerData.silver -= silver;
                    }
                    if (gold != 0)
                    {
                        debug += "\nlost " + gold + " gold";
                        PlayerDataManager.playerData.gold -= gold;
                    }

                    if (PlayerManagerUI.Instance != null)
                        PlayerManagerUI.Instance.UpdateDrachs();

                    Debug.Log(debug);

                    callback(null);
                }
                else
                {
                    callback(response);
                }
            });
        }

        public static IngredientBundleData GetBundle(string id)
        {
            if (BundleDict.ContainsKey(id))
            {
                return BundleDict[id];
            }
            else
            {
                Debug.LogError($"ingredient bundle not found (\"{id}\")");
                return new IngredientBundleData();
            }
        }

        public static ConsumableData GetConsumable(string id)
        {
            if (ConsumableDict.ContainsKey(id))
            {
                return ConsumableDict[id];
            }
            else
            {
                Debug.LogError($"consumable not found (\"{id}\")");
                return new ConsumableData();
            }
        }

        public static SilverBundleData GetSilverBundle(string id)
        {
            if (SilverBundleDict.ContainsKey(id))
            {
                return SilverBundleDict[id];
            }
            else
            {
                Debug.LogError($"silver bundle not found (\"{id}\")");
                return new SilverBundleData();
            }
        }

        public static void SetupOldStore(StoreData data)
        {
            m_OldStore = new StoreApiObject();
            char gender = PlayerDataManager.playerData.male ? 'm' : 'f';

            //setup bundles
            m_OldStore.bundles = new List<StoreApiItem>();
            foreach (StoreItem item in data.Bundles)
            {
                IngredientBundleData bundle = GetBundle(item.id);

                StoreApiItem aux = new StoreApiItem();
                aux.id = item.id;
                aux.type = "bundles";
                aux.silver = bundle.silver;
                aux.gold = bundle.gold;

                if (bundle.collectables != null)
                {
                    aux.contents = new List<StoreItemContent>();
                    for (int i = 0; i < bundle.collectables.Length; i++)
                    {
                        aux.contents.Add(new StoreItemContent()
                        {
                            id = bundle.collectables[i],
                            count = bundle.amount[i]
                        });
                    }
                }

                m_OldStore.bundles.Add(aux);
            }

            //setup consumables
            m_OldStore.consumables = new List<StoreApiItem>();
            foreach (StoreItem item in data.Consumables)
            {
                ConsumableData consumable = GetConsumable(item.id);

                StoreApiItem aux = new StoreApiItem();
                aux.id = item.id;
                aux.type = "consumables";
                aux.silver = consumable.silver;
                aux.gold = consumable.gold;

                OldStore.consumables.Add(aux);
            }

            //setup silver
            m_OldStore.silver = new List<StoreApiItem>();
            foreach (StoreItem item in data.Silver)
            {
                SilverBundleData silver = GetSilverBundle(item.id);

                StoreApiItem aux = new StoreApiItem();
                aux.id = item.id;
                aux.type = "silver";
                aux.amount = silver.amount;
                aux.bonus = silver.extra;
                aux.cost = silver.cost;
                aux.productId = silver.product;

                m_OldStore.silver.Add(aux);
            }

            //setup cosmetics
            m_OldStore.cosmetics = new List<CosmeticData>();
            foreach(StoreItem item in data.Cosmetics)
            {
                CosmeticData aux = DownloadedAssets.GetCosmetic(item.id);

                if (aux == null)
                    continue;

                if (aux.type[0] != gender)
                    continue;

                aux.unlockOn = item.unlockOn;
                aux.tooltip = item.tooltip;
                m_OldStore.cosmetics.Add(aux);
            }

            //setup styles
            m_OldStore.styles = new List<CosmeticData>();
            foreach (StoreItem item in data.Styles)
            {
                CosmeticData aux = DownloadedAssets.GetCosmetic(item.id);

                if (aux == null)
                    continue;

                if (aux.type[0] != gender)
                    continue;

                aux.unlockOn = item.unlockOn;
                aux.tooltip = item.tooltip;
                m_OldStore.styles.Add(aux);
            }
        }
    }
}
