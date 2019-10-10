using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Raincrow.Store
{
    public struct ConsumableData
    {
        public float duration;
    }

    public struct CurrencyBundleData
    {
        [DefaultValue("")]
        public string product;
        public float cost;
        [JsonProperty("silverAmount")]
        public int silver;
        [JsonProperty("goldAmount")]
        public int gold;
        [JsonProperty("silverBonus")]
        public int silverBonus;
        [JsonProperty("goldBonus")]
        public int goldBonus;
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

    public class StoreData
    {
        public List<StoreItem> Bundles;
        public List<StoreItem> Consumables;
        public List<StoreItem> Currencies;
        public List<StoreItem> Cosmetics;
        public List<StoreItem> Styles;

        public int GetPrice(string type, string id, bool IsSilver)
        {
            List<StoreItem> items = null;
            switch (type)
            {
                case StoreManagerAPI.TYPE_CURRENCY :            items = new List<StoreItem>(Currencies); break;
                case StoreManagerAPI.TYPE_COSMETIC :            items = new List<StoreItem>(Cosmetics); items.AddRange(Styles); break;
                case StoreManagerAPI.TYPE_INGREDIENT_BUNDLE :   items = new List<StoreItem>(Bundles); break;
                case StoreManagerAPI.TYPE_ELIXIRS :             items = new List<StoreItem>(Consumables); break;
            }

            if (items == null)
                return 0;

            foreach (var item in items)
            {
                if (item.id == id)
                    return IsSilver ? item.silver : item.gold;
            }

            return 0;
        }
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
        public const string TYPE_CURRENCY = "currency";
        public const string TYPE_COSMETIC = "cosmetics";
        public const string TYPE_INGREDIENT_BUNDLE = "bundles";
        public const string TYPE_ELIXIRS = "consumables";

        public static StoreData StoreData { get; set; }
        
        public static Dictionary<string, List<ItemData>> BundleDict { get; set; }
        public static Dictionary<string, ConsumableData> ConsumableDict { get; set; }
        public static Dictionary<string, CurrencyBundleData> CurrencyBundleDict { get; set; }

        public static System.Action<string, string> OnPurchaseComplete;


        public static ItemData[] GetIngredientBundle(string id)
        {
            if (BundleDict.ContainsKey(id))
            {
                return BundleDict[id].ToArray();
            }
            else
            {
                LogError($"ingredient bundle not found (\"{id}\")");
                return new ItemData[0];
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
                LogError($"consumable not found (\"{id}\")");
                return new ConsumableData();
            }
        }

        public static CurrencyBundleData GetCurrencyBundle(string id)
        {
            if (CurrencyBundleDict.ContainsKey(id))
            {
                return CurrencyBundleDict[id];
            }
            else
            {
                LogError($"currency bundle not found (\"{id}\")");
                return new CurrencyBundleData();
            }
        }

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

            Log("purchasing \"" + id + "\"");

            APIManager.Instance.Post("shop/purchase", JsonConvert.SerializeObject(data), (response, result) =>
            {
                if (result == 200)
                {
                    string debug = "purchase complete:\n";
                    debug += $"[{type}] {id}";

                    int silver = 0;
                    int gold = 0;

                    switch (type)
                    {
                        case TYPE_CURRENCY:
                            //currency is processed on IAPSilver
                            break;

                        case TYPE_COSMETIC:

                            //item = Store.Cosmetics.Find(cos => cos.id == id);
                            CosmeticData cosmetic = DownloadedAssets.GetCosmetic(id);

                            //get the price
                            if (currency == "silver")
                                silver = StoreData.GetPrice(type, id, true);
                            if (currency == "gold")
                                gold = StoreData.GetPrice(type, id, false);

                            //add the item to inventory
                            PlayerDataManager.playerData.inventory.cosmetics.Add(cosmetic);
                            break;

                        case TYPE_INGREDIENT_BUNDLE:

                            ItemData[] bundle = StoreManagerAPI.GetIngredientBundle(id);

                            //get the price
                            if (currency == "silver")
                                silver = StoreData.GetPrice(type, id, true);
                            if (currency == "gold")
                                gold = StoreData.GetPrice(type, id, false);

                            //add the ingredients to the inventory
                            for (int i = 0; i < bundle.Length; i++)
                            {
                                debug += "\n\t" + bundle[i].id+ " +" + bundle[i].count;
                                PlayerDataManager.playerData.AddIngredient(bundle[i].id, bundle[i].count);
                            }
                            break;

                        case TYPE_ELIXIRS:

                            Item elixir = PlayerDataManager.playerData.inventory.consumables.Find(it => it.id == id);
                            ConsumableData consumable = StoreManagerAPI.GetConsumable(id); ;

                            //get the price
                            if (currency == "silver")
                                silver = StoreData.GetPrice(type, id, true);
                            if (currency == "gold")
                                gold = StoreData.GetPrice(type, id, false);

                            //add to the inventory
                            if (elixir == null || string.IsNullOrEmpty(elixir.id))
                            {
                                PlayerDataManager.playerData.inventory.consumables.Add(new Item
                                {
                                    id = id,
                                    count = 1
                                });
                            }
                            else
                            {
                                elixir.count += 1;
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

                    Log(debug);

                    callback?.Invoke(null);
                    OnPurchaseComplete?.Invoke(id, type);
                }
                else
                {
                    callback?.Invoke(response);
                }
            });
        }


        private static void Log(string msg)
        {
#if UNITY_EDITOR
            Debug.Log("[<color=cyan>StoreAPI</color>] " + msg);
            return;
#endif
            Debug.Log("[StoreAPI] " + msg);
        }

        private static void LogError(string msg)
        {
#if UNITY_EDITOR
            Debug.LogError("[<color=cyan>StoreAPI</color>] " + msg);
            return;
#endif
            Debug.LogError("[StoreAPI] " + msg);
        }
    }
}
